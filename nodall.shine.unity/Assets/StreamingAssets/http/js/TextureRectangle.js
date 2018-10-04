/*
 * Transformable container base class for transforming shapes
 * keeps track of fingers, sub-classes can calculate values based on finger positions
 *
 * dispatches events:
 * start, update, complete
 */


TextureRectangle = (function(){
  var Parent = createjs.Container;

  // Stack x finger movements together for fading out transforms
  var STACK = 5;
  var START = 'start';
  var UPDATE = 'update';
  var COMPLETE = 'complete';

  var TextureRectangle = function(){
    // reference to instance
    var self = this;
    self.typeOf = 'TextureRectangle';

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

    self.width = 160
    self.height = 100


    var _changed = false;

    // constructor
    var Init = function(){
      // call super constructor
      if (Parent) Parent.call(self);

      self._initialized = true;

      self.addEventListener('mousedown', _mousedown);
      self.addEventListener('pressmove', _pressmove);
      self.addEventListener('pressup', _pressup);
      self.addEventListener('tick', _enterFrame);


        /*
        self.image = new createjs.Shape() //new createjs.Bitmap("Grid.png")
        self.image.alpha = 0.1
        self.addChild(self.image)
        */

        self.rectShape = new createjs.Shape();
        self.rectShape.graphics.beginStroke("#444").drawRect(0, 0, self.width, self.height)
        self.addChild(self.rectShape)

        var w2 = self.width / 2
        var h2 = self.height / 2


        self.circleShapes = []
        for (var i = 0; i < 4; ++i) {
            var cir = new createjs.Shape()            
            cir.borderIndex = i
            self.circleShapes.push(cir)
            cir.graphics.beginFill("#444").drawCircle(0, 0, 10)

            cir.hitArea = new createjs.Shape()

            self.addChild(cir)

            cir.addEventListener("mousedown", function(event) {
                console.log("down in circle")
                var target = event.target


                  var local = self.stage.globalToLocal(event.stageX, event.stageY);

                  //local.x = event.stageX
                  //local.y = event.stageY

                target._offset={x:target.x-local.x,y:target.y-local.y};
                //target._offset={x:target.x-event.stageX,y:target.y-event.stageY};

            })

            cir.addEventListener("pressmove", function(event) {


              var local = self.stage.globalToLocal(event.stageX, event.stageY);

              console.log("pressing in circle "+event.target.x+", "+event.target.y)

              if (self.isSnapEnabled) {
                if (event.target.borderIndex == 1 || event.target.borderIndex == 3) {
                  event.target.y = event.target.stage.snapToY(event.stageY + event.target._offset.y);
                }
                else {
                  event.target.x = event.target.stage.snapToX(event.stageX + event.target._offset.x);
                }
              }

              event.target.x = local.x + event.target._offset.x;
              event.target.y = local.y + event.target._offset.y;
              
              self.updateFromView(true)
            })
        }

        self.circleShapes[0].y = h2;
        self.circleShapes[1].x = w2;
        self.circleShapes[2].x = self.width;
        self.circleShapes[2].y = h2;
        self.circleShapes[3].x = w2;
        self.circleShapes[3].y = self.height;


        self.text = new createjs.Text("", "20px Arial", "#222")
        self.text.textBaseline = "center"
        self.text.textAlign = "center"
        self.addChild(self.text)

        self.text.x = w2
        self.text.y = h2

    };

    self.setup = function(image)
    {
        var img = new Image()
        img.onload = self.updateFromViewSilent
        self.image = new createjs.Bitmap(img)
        img.src = image
        self.image.alpha = 0.8
        self.addChildAt(self.image, 1)

    }

    self.updateFromViewSilent = function() {
      self.updateFromView()
    }

    self.setText = function(text) {
        self.text.text = text
    }

    self.parse = function(bezstr) {
      var pt = bezstr.split(";")
      self.circleShapes[0].x = parseFloat(pt[0])*self.stage.unitX
      self.circleShapes[1].y = parseFloat(pt[1])*self.stage.unitY
      self.circleShapes[2].x = parseFloat(pt[2])*self.stage.unitX
      self.circleShapes[3].y = parseFloat(pt[3])*self.stage.unitY

      self.updateFromView()

    }

    self.toString = function() {
      return (self.circleShapes[0].x/self.stage.unitX) +";" + (self.circleShapes[1].y/self.stage.unitY) + ";" + (self.circleShapes[2].x/self.stage.unitX) + ";" +  (self.circleShapes[3].y/self.stage.unitY);
    }

    self.updateFromView = function(notify) {

      var rectShapeX = self.circleShapes[0].x
      var rectShapeY = self.circleShapes[1].y

      self.width = self.circleShapes[2].x - self.circleShapes[0].x
      self.height = self.circleShapes[3].y - self.circleShapes[1].y

        var w2 = self.width / 2
        var h2 = self.height / 2

        self.text.x = rectShapeX + w2
        self.text.y = rectShapeY + h2

        self.circleShapes[0].y = h2 + rectShapeY
        self.circleShapes[1].x = w2 + rectShapeX        
        self.circleShapes[2].y = h2 + rectShapeY
        self.circleShapes[3].x = w2 + rectShapeX

        if (self.image !== undefined) {
          var iw = self.image.image.width
          var ih = self.image.image.height

          if (iw == 0) iw = 1000
          if (ih == 0) ih = 1000

          self.image.x = rectShapeX
          self.image.y = rectShapeY
          self.image.scaleX = self.width/iw
          self.image.scaleY = self.height/ih
        }

      self.snapList = [ { x: rectShapeX, y: rectShapeY }, { x: rectShapeX + self.width, y: rectShapeY + self.height }]
      if (self.isEnabled)
        self.snapList = []

      self.rectShape.graphics.clear().beginStroke("#444").drawRect(rectShapeX, rectShapeY, self.width, self.height)

      console.log(self.width+"x"+self.height+" at "+rectShapeX+", "+rectShapeY)

      self.stage.update()
      if (typeof(notify) !== 'undefined' && typeof(self.onChange) == 'function')
        self.onChange(self, self.UID, self.toString())
    }

    self.disable = function(value) {
      self.isEnabled = false
      self.alpha = 0.5
      var size = (self.stage.scaleX + 2) / 3;      
      for (var i = 0; i < 4; ++i) self.circleShapes[i].graphics.clear().beginFill("#444").drawCircle(0, 0, 5/size)
      for (var i = 0; i < 4; ++i) self.circleShapes[i].hitArea.graphics.clear().beginFill("#444").drawCircle(0, 0, 5/size)
      self.stage.setChildIndex(self, 1)
      self.stage.update()
    }

    self.enable = function(value) {
      self.isEnabled = true
      self.alpha = 1.0
      //self.image.alpha = 1.0
      var size = (self.stage.scaleX + 2) / 3;      
      for (var i = 0; i < 4; ++i) self.circleShapes[i].graphics.clear().beginFill("#444").drawCircle(0, 0, 10/size)
      for (var i = 0; i < 4; ++i) self.circleShapes[i].hitArea.graphics.clear().beginFill("#444").drawCircle(0, 0, 20/size)

      self.stage.setChildIndex(self, self.stage.getNumChildren() - 1)
      self.stage.update()
    }    

    self.recreateAll = function() {
      var size = (self.stage.scaleX + 2) / 3;
      if (self.isEnabled) {
        for (var i = 0; i < 4; ++i) self.circleShapes[i].graphics.clear().beginFill("#444").drawCircle(0, 0, 10/size)
        for (var i = 0; i < 4; ++i) self.circleShapes[i].hitArea.graphics.clear().beginFill("#444").drawCircle(0, 0, 20/size)
      } else {      
        for (var i = 0; i < 4; ++i) self.circleShapes[i].graphics.clear().beginFill("#444").drawCircle(0, 0, 5/size)
        for (var i = 0; i < 4; ++i) self.circleShapes[i].hitArea.graphics.clear().beginFill("#444").drawCircle(0, 0, 5/size)
      }
    }

    self.setLeft = function(value) { self.setBorder(0, value) }
    self.setTop = function(value) { self.setBorder(1, value) }
    self.setRight = function(value) { self.setBorder(2, value) }
    self.setBottom = function(value) { self.setBorder(3, value) }


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
  TextureRectangle.prototype = {};

  // extend Transformable with defined parent
  if (Parent) sys.inherits(TextureRectangle, Parent);

  // return Transformable definition to public scope
  return TextureRectangle;
})();