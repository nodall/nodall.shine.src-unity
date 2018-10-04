/*
 * Transformable container base class for transforming shapes
 * keeps track of fingers, sub-classes can calculate values based on finger positions
 *
 * dispatches events:
 * start, update, complete
 */


Bezier = (function(){
  var Parent = createjs.Container;

  // Stack x finger movements together for fading out transforms
  var STACK = 5;
  var START = 'start';
  var UPDATE = 'update';
  var COMPLETE = 'complete';

  var Bezier = function(){

    // reference to instance
    var self = this;
    self.typeOf = 'Bezier';

    // numeric value to snap transformation by x
    self.snap = {};
    self.isSnapEnabled = false;

    // freely transformable by property, overwrites borders if true
    self.free = {};

    // borders for specific values get stored here
    self.borders = {};

    // lock transformations regardless of other values
    self.lock = false;


    // protected values
    // NOTE: read-only, don't modify from outside!
    // currently active fingers
    self._activeFingers = 0;

    // finger positions
    self._fingers = [];

    self.width = 320
    self.height = 200    

    self.isEnabled = true


    var _changed = false;

    // constructor
    var Init = function(){
      // call super constructor
      if (Parent) Parent.call(self);

  //    alert('initing bezier')
      self._initialized = true;

      self.addEventListener('mousedown', _mousedown);
      self.addEventListener('pressmove', _pressmove);
      self.addEventListener('pressup', _pressup);
      self.addEventListener('tick', _enterFrame);

        var gw = self.width/3
        var gh = self.height/3
        self.lines = []
        self.linesArray = []

        for (var y = 0; y < 4; ++y) {
          for (var x = 0; x < 4; ++x) {

            if (x < 3) {
              var line = new createjs.Shape()
              line.update = function(line) { line.graphics.clear().beginStroke("#222").moveTo(line.x1, line.y1).lineTo(line.x2, line.y2) }
              line.x1 = x*gw
              line.y1 = y*gh
              line.x2 = (x+1)*gw
              line.y2 = y*gh
              line.update(line)
              //console.log("H "+Math.floor(line.x1)+", "+Math.floor(line.y1)+"  to  "+Math.floor(line.x2)+", "+Math.floor(line.y2))
              //line.graphics.beginStroke("#222").moveTo(x*gw, y*gh).lineTo((x+1)*gw, y*gh)
              self.addChild(line)
              self.lines[x+"h"+y] = line
              self.linesArray.push(line)
            }

            if (y < 3) {
              var line = new createjs.Shape()
              line.update = function(line) { line.graphics.clear().beginStroke("#222").moveTo(line.x1, line.y1).lineTo(line.x2, line.y2) }
              line.x1 = x*gw
              line.y1 = y*gh
              line.x2 = x*gw
              line.y2 = (y+1)*gh
              //console.log("V "+Math.floor(line.x1)+", "+Math.floor(line.y1)+"  to  "+Math.floor(line.x2)+", "+Math.floor(line.y2))
              //line.graphics.beginStroke("#222").moveTo(x*gw, y*gh).lineTo(x*gw, (y+1)*gh)
              line.update(line)
              self.addChild(line)
              self.lines[x+"v"+y] = line
              self.linesArray.push(line)
            }
        }
      }


        self.circleShapes = []
        for (var y = 0; y < 4; ++y) {
          for (var x = 0; x < 4; ++x) {
              var cir = new createjs.Shape()
              cir.hitArea = new createjs.Shape()
              cir.circleX = x
              cir.circleY = y

              cir.x = x * gw
              cir.y = y * gh
              self.circleShapes.push(cir)
              //cir.graphics.beginFill("#444").drawCircle(0, 0, 10)
              self.addChild(cir)

              cir.addEventListener("mousedown", function(event) {
                  //console.log("down in circle")
                  var target = event.target

                  var local = self.stage.globalToLocal(event.stageX, event.stageY);

                  //local.x = event.stageX
                  //local.y = event.stageY

                  target._offset={x:target.x-local.x,y:target.y-local.y};


              })

              cir.addEventListener("pressmove", function(event) {
                //console.log("pressing in circle ")

                  var local = self.stage.globalToLocal(event.stageX, event.stageY);

                  //local.x = event.stageX
                  //local.y = event.stageY

                  event.target.x = local.x + event.target._offset.x;
                  event.target.y = local.y + event.target._offset.y;

                  if (self.isSnapEnabled) {
                    var pt = event.target.stage.snapToXY(event.target.x, event.target.y)

                    event.target.x = pt.x
                    event.target.y = pt.y
                  }

                  var line1 = self.lines[event.target.circleX+"h"+event.target.circleY]
                  if (line1) {
                      line1.x1 = event.target.x
                      line1.y1 = event.target.y
                      line1.update(line1)
                  }
                  var line2 = self.lines[event.target.circleX+"v"+event.target.circleY]
                  if (line2) {
                      line2.x1 = event.target.x
                      line2.y1 = event.target.y
                      line2.update(line2)
                  }
                  var line3 = self.lines[(event.target.circleX-1)+"h"+event.target.circleY]
                  if (line3) {
                      line3.x2 = event.target.x
                      line3.y2 = event.target.y
                      line3.update(line3)
                  }
                  var line4 = self.lines[event.target.circleX+"v"+(event.target.circleY-1)]
                  if (line4) {
                      line4.x2 = event.target.x
                      line4.y2 = event.target.y
                      line4.update(line4)
                  }

                self.updateFromView(true)
              })
            }
        }
    };

    self.setText = function(text) {
        self.text.text = text
    }

    self.parse = function(bezstr) {
      var spl = bezstr.split("|")
      for (var i = 0; i < 16; ++i) {
        var pt = spl[i].split(";")
        self.circleShapes[i].x = parseFloat(pt[0])*self.stage.unitX
        self.circleShapes[i].y = parseFloat(pt[1])*self.stage.unitY
      }

      self.updateLines()

      self.stage.update()
    }

    self.recreateAll = function() {
      var isQuad = self.surface.type == 'quad' || self.surface.type == 'perspective'
      for (var i = 0; i < 16; ++i) {
        var circle = self.circleShapes[i]
        var cs = self.circleShapes[i]
        var size = (self.stage.scaleX + 2) / 3;
        if (self.isEnabled) {
          circle.alpha = 1
          cs.graphics.clear().beginFill("#444").drawCircle(0, 0, 10 / size)
          cs.hitArea.graphics.clear().beginFill("#444").drawCircle(0, 0, 20 / size)
        } else {
          circle.alpha = 0.2
          cs.graphics.clear().beginFill("#444").drawCircle(0, 0, 2 / size)
          cs.hitArea.graphics.clear().beginFill("#444").drawCircle(0, 0, 2 / size)
        }

        if (isQuad && i != 0 && i != 3 && i != 12 && i != 15) {
          cs.graphics.clear()
          cs.hitArea.graphics.clear()
        }        
      }

      for (var i = 0; i < self.linesArray.length; ++i) {
        var line = self.linesArray[i]
        var lg = line.graphics
        if (self.isEnabled) {
          line.alpha = 0.9
          lg.clear().beginStroke("#222").moveTo(line.x1, line.y1).lineTo(line.x2, line.y2)
        } else {
          line.alpha = 0.2
          lg.clear().beginStroke("#222").moveTo(line.x1, line.y1).lineTo(line.x2, line.y2)
        }
      }
    }

    self.toString = function() {
      var str = ""
      for (var i = 0; i < 16; ++i) {
        str += (self.circleShapes[i].x/self.stage.unitX)+";"+(self.circleShapes[i].y/self.stage.unitY) + (i == 15 ? "": "|")
      }

      return str + "|0;0;1;1";
    }

    self.updateLines = function() {
      for (var y = 0; y < 4; ++y) {
        for (var x = 0; x < 4; ++x) {
            var cir = self.circleShapes[x+y*4]
            var line1 = self.lines[cir.circleX+"h"+cir.circleY]
            if (line1) {
                line1.x1 = cir.x
                line1.y1 = cir.y
                line1.update(line1)
            }
            var line2 = self.lines[cir.circleX+"v"+cir.circleY]
            if (line2) {
                line2.x1 = cir.x
                line2.y1 = cir.y
                line2.update(line2)
            }
            var line3 = self.lines[(cir.circleX-1)+"h"+cir.circleY]
            if (line3) {
                line3.x2 = cir.x
                line3.y2 = cir.y
                line3.update(line3)
            }
            var line4 = self.lines[cir.circleX+"v"+(cir.circleY-1)]
            if (line4) {
                line4.x2 = cir.x
                line4.y2 = cir.y
                line4.update(line4)
            }
         }
      }
    }

    self.updateFromView = function(notify) {

      //console.log(self.UID+" - "+self.Name)

      var isQuad = self.surface.type == 'quad' || self.surface.type == 'perspective'
      if (isQuad) {
        self.filterQuadPoints();
        self.updateLines()
        self.recreateAll()
        self.stage.update()
      }

      self.snapList = []
      for (var i = 0; i < 16; ++i) {
        if (window.isSnapEnabled) {
          self.snapList.push({point: true, x: self.circleShapes[i].x, y: self.circleShapes[i].y})
        }
      }

      self.stage.update()
      if (typeof(notify) !== 'undefined' && typeof(self.onChange) == 'function')
        self.onChange(self, self.UID, self.toString())
    }

    self.disable = function(value) {
      self.isEnabled = false
      self.recreateAll()
      self.stage.setChildIndex(self, 1)
      self.stage.update()
    }

    self.enable = function(value) {
      self.isEnabled = true
      self.recreateAll()
      self.stage.setChildIndex(self, self.stage.getNumChildren() - 1)
      self.stage.update()
    }

    self.reset = function() {
      self.parse(".000000000;.000000000|.333333333;.000000000|.666666667;.000000000|1.000000000;.000000000|.000000000;.333333333|.333333333;.333333333|.666666667;.333333333|1.000000000;.333333333|.00000000;.666666667|.333333333;.666666667|.666666667;.666666667|1.000000000;.666666667|.000000000;1.000000000|.333333333;1.000000000|.666666667;1.000000000|1.000000000;1.000000000|0;0;1;1")
    }

    self.filterQuadPoints = function() {

      var obj = []
      for (var i = 0; i < 16; ++i) {
        obj[i] = { X: self.circleShapes[i].x,  Y: self.circleShapes[i].y }
      }
      var quad = [ obj[0], obj[3], obj[15], obj[12] ]
      var dx = 1 / 3 //(rectangle.right - rectangle.left) / 3
      var dy = 1 / 3 //(rectangle.top - rectangle.bottom) / 3

      for (var i = 0; i < 4*4; ++i) {

        var x = i % 4
        var y = Math.floor(i / 4)

        var origValue = {
          X: x * dx, 
          Y: y * dy }; //[ rectangle.left + x * dx, rectangle.top - y * dy ];

        if (self.surface.type == 'perspective') obj[i] = self.mathToolPerspectiveCorrection(quad, origValue)
        else obj[i] = self.mathToolInterpolateInQuad(quad, origValue)
      }

      for (var i = 0; i < 16; ++i) {
        self.circleShapes[i].x = obj[i].X
        self.circleShapes[i].y = obj[i].Y
      }    
    }

    self.mathToolPerspectiveCorrection = function(quad, position) {

      var vA1 = {
        X: quad[1].X - quad[2].X,
        Y: quad[1].Y - quad[2].Y };
      var vA2 = {
        X: quad[3].X - quad[2].X,
        Y: quad[3].Y - quad[2].Y };
      var vE = {
        X: quad[0].X - quad[1].X + quad[2].X - quad[3].X,
        Y: quad[0].Y - quad[1].Y + quad[2].Y - quad[3].Y };        

      var den = ((vA1.X * vA2.Y) - (vA2.X * vA1.Y));

      if (0.0 == den)
      {
          den = 0.000001;
      }

      var g = ((vE.X * vA2.Y) - (vA2.X * vE.Y)) / den;
      var h = ((vA1.X * vE.Y) - (vE.X * vA1.Y)) / den;

      var a = (quad[1].X - quad[0].X) + (g * quad[1].X);
      var d = (quad[1].Y - quad[0].Y) + (g * quad[1].Y);

      var b = (quad[3].X - quad[0].X) + (h * quad[3].X);
      var e = (quad[3].Y - quad[0].Y) + (h * quad[3].Y);

      var c = quad[0].X;
      var f = quad[0].Y;

      den = ((position.X * g) + (position.Y * h) + 1.0);

      if (0.0 == den)
      {
        den = 0.00001;
      }

      return { 
        X: ((position.X * a) + (position.Y * b) + c) / den,
        Y: ((position.X * d) + (position.Y * e) + f) / den };
    }

    self.mathToolInterpolateInQuad = function(quad, position) {
      var A = quad[0]; var B = quad[1]; var C = quad[3]; var D = quad[2];
      var t = position.X; var s = position.Y;
      var E = {
       X: A.X + t * (B.X - A.X),
       Y: A.Y + t * (B.Y - A.Y) };
      var F = {
       X: C.X + t * (D.X - C.X),
       Y: C.Y + t * (D.Y - C.Y) };

      return { 
        X: E.X + s * (F.X - E.X),
        Y: E.Y + s * (F.Y - E.Y) };
    };


    // store initial touchpoint-position
    var _mousedown = function(event){
      if (! event.pointerID) event.pointerID = -1;

      self._fingers[event.pointerID] = {
        start: {x: event.stageX, y: event.stageY},
        current: {x: event.stageX, y: event.stageY},
        old: {x: event.stageX, y: event.stageY}
      };

      _calculateActiveFingers();

      self.dispatchEvent(START);
    };

    // update touchpoint-positions
    var _pressmove = function(event){
      if (! event.pointerID) event.pointerID = -1;

      self._fingers[event.pointerID].current.x = event.stageX;
      self._fingers[event.pointerID].current.y = event.stageY;

      _calculateActiveFingers();

      _changed = true;
    };

    // if positions changed (through pressmove): dispatch update-event for later usage and keep track of old point-position
    // dispatch updates only on tick to save some performance
    var _enterFrame = function(){
      if (_changed) {
        _changed = false;
        self.dispatchEvent(UPDATE);

        for (var pointerID in self._fingers) {
          if (self._fingers[pointerID].start) {
            self._fingers[pointerID].old.x = self._fingers[pointerID].current.x;
            self._fingers[pointerID].old.y = self._fingers[pointerID].current.y;
          }
        }
      }
    };

    // delete old and unused finger-positions
    var _pressup = function(event){
      if (! event.pointerID) event.pointerID = -1;

      if (self._fingers[event.pointerID]) {
        delete(self._fingers[event.pointerID]);
      }

      _calculateActiveFingers();

      self.dispatchEvent(COMPLETE);
    };

    // calculates currently active fingers, can be used later in subclasses
    var _calculateActiveFingers = function(){
      self._activeFingers = 0;

      for (var pointerID in self._fingers) {
        if (self._fingers[pointerID].start) {
          self._activeFingers++;
        }
      }
    };

    // initialize instance
    Init();
  };

  // export public Transformable definition
  Bezier.prototype = {};

  // extend Transformable with defined parent
  if (Parent) sys.inherits(Bezier, Parent);

  // return Transformable definition to public scope
  return Bezier;
})();