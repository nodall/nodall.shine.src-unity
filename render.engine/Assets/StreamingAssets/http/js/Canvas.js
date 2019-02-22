//var bezier;

  function initCanvas(canvasId, offsetX, offsetY) {

  if (offsetY === undefined) {
    offsetX = 0
    offsetY = 0
  }


  var stage;
  var tr1, tr2;
  var backgroundRect;

var canvas = document.getElementById(canvasId);
canvas.width = window.innerWidth - 20;
canvas.height = window.innerHeight/2;
stage = new createjs.Stage(canvasId);


function addRect(x, y, w, h) {
  var g = new createjs.Graphics().beginStroke("#ff0000").drawRect(x, y, w, h);
  var s = new createjs.Shape(g);
  s.x = x;
  s.y = y;
  stage.addChild(s);
  stage.update();
  return s;
}

function updateRect(shape, x, y, w, h) {
  s.graphics.clear().beginStroke("#ff0000").drawRect(x, y, w, h);
  stage.update()
}

stage.offsetX = offsetX
stage.offsetY = offsetY

stage.unitX = canvas.width*0.8
stage.unitY = canvas.height*0.8

stage.x = canvas.width*0.1 + 2*stage.offsetX * stage.unitX
stage.y = canvas.height*0.1 + 2*stage.offsetY * stage.unitY

backgroundRect = addRect(-stage.offsetX*stage.unitX, -stage.offsetY*stage.unitY, stage.unitX, stage.unitY)
backgroundRect.name = "backgroundRect"
backgroundRect.snapList = [ { x:0, y:0 }, { x:stage.unitX, y:stage.unitY } ]

stage._fingers = []
stage._activeFingers = 0

stage.snapDistance = 6

stage.snapToX = function(x) {

  var children = stage.children;

  for (var i = 0; i < children.length; ++i) {
    var snapList = children[i].snapList

    if (snapList) {
      for (var j = 0; j < snapList.length; ++j) {
        var pt = snapList[j]
        if (x > pt.x - stage.snapDistance && x < pt.x + stage.snapDistance)
          x = pt.x
      }
    }
  }
  return x;
}

stage.snapToY = function(y) {
  var children = stage.children;

  for (var i = 0; i < children.length; ++i) {
    var snapList = children[i].snapList

    if (snapList) {
      for (var j = 0; j < snapList.length; ++j) {
        var pt = snapList[j]
        if (y > pt.y - stage.snapDistance && y < pt.y + stage.snapDistance)
          y = pt.y
      }
    }
  }
  return y;
}

stage.snapToXY = function(x, y) {

  var children = stage.children;

  for (var i = 0; i < children.length; ++i) {
    var snapList = children[i].snapList

    if (snapList) {
      for (var j = 0; j < snapList.length; ++j) {
        var pt = snapList[j]

        if (pt.point) {
          if (x > pt.x - stage.snapDistance && x < pt.x + stage.snapDistance && y > pt.y - stage.snapDistance && y < pt.y + stage.snapDistance)
          {
            x = pt.x
            y = pt.y
          }
        } else {

          if (x > pt.x - stage.snapDistance && x < pt.x + stage.snapDistance)
            x = pt.x

          if (y > pt.y - stage.snapDistance && y < pt.y + stage.snapDistance)
            y = pt.y

        }
      }
    }    
  }
  return { x: x, y: y };
}



stage._calculateActiveFingers = function(){
      this._activeFingers = 0;

      for (var pointerID in this._fingers) {
        if (this._fingers[pointerID].start) {
          this._activeFingers++;
        }
      }
    };

var _getDistance = function(p1, p2) {
  var x = p2.x - p1.x;
  var y = p2.y - p1.y;

  return Math.sqrt((x * x) + (y * y));
};


var recreateAllGraphics = function() {

  for (var i = 1; i < stage.numChildren; ++i) {
    var ch = stage.getChildAt(i);
    if ( ch.recreateAll != undefined) {
        ch.recreateAll();
    }
  }
}

var mouseWheelHandler = function(e) {

	if(Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)))>0)
		zoom=1.1;
	else
		zoom=1/1.1;

  if ((zoom > 1 && stage.scaleX < 35) || (zoom < 1 && stage.scaleX > 0.5)) {
    
    var local = stage.globalToLocal(stage.mouseX, stage.mouseY);
    stage.regX=local.x;
    stage.regY=local.y;
    stage.x=stage.mouseX;
    stage.y=stage.mouseY;	
    stage.scaleX=stage.scaleY*=zoom;

    recreateAllGraphics();
    stage.update();
  }
}

canvas.addEventListener('mousewheel', mouseWheelHandler, false);
canvas.addEventListener('DOMMouseScroll', mouseWheelHandler, false);


stage.addEventListener("stagemousedown", function(event) {
  //if (! event.pointerID) event.pointerID = -1;

  stage._hasEndedAction = false;
  var local = stage.globalToLocal(event.stageX, event.stageY);    
  var objUnder = stage.getObjectUnderPoint(local.x, local.y)
  console.log(objUnder)
  if (objUnder != null && objUnder.image == undefined) {
    console.log('under object')
    return;
  }

  this._offset={x:stage.x-event.stageX,y:stage.y-event.stageY};  
  

      stage._fingers[event.pointerID] = {
        start: {x: event.stageX, y: event.stageY},
        current: {x: event.stageX, y: event.stageY},
        old: {x: event.stageX, y: event.stageY}
      };

      stage._calculateActiveFingers();
}); 

  stage.addEventListener("stagemousemove", function(event) {

      //if (! event.pointerID) event.pointerID = -1;

    if (stage._hasEndedAction == true || stage._activeFingers == 0) return;

      stage._fingers[event.pointerID].old.x = stage._fingers[event.pointerID].current.x;
      stage._fingers[event.pointerID].old.y = stage._fingers[event.pointerID].current.y;

      stage._fingers[event.pointerID].current.x = event.stageX;
      stage._fingers[event.pointerID].current.y = event.stageY;


    if (stage._activeFingers == 1) {
      stage.x = event.stageX+this._offset.x;
      stage.y = event.stageY+this._offset.y;
      stage.update();
    } else if (stage._activeFingers == 2) {

      var points = [];

        for (var k in stage._fingers) {
          if (stage._fingers[k].current) {
            points.push(stage._fingers[k]);
            if (points.length >= 2) break;
          }
        }

        var scale = _getDistance(points[0].current, points[1].current) / _getDistance(points[0].old, points[1].old);

        var center = { x: (points[0].current.x + points[1].current.x) / 2,
                        y: (points[0].current.y + points[1].current.y) / 2 };

  var local = stage.globalToLocal(stage.mouseX, stage.mouseY);
  stage.regX = local.x;
  stage.regY = local.y;
  stage.x = stage.mouseX;
  stage.y = stage.mouseY;


      if ((scale > 1 && stage.scaleX < 35) || (scale < 1 && stage.scaleX > 0.5)) {

          stage.scaleX *= scale;
          stage.scaleY *= scale;
      }


        var point1 = points[0].old;
        var point2 = points[1].old;
        var startAngle = Math.atan2((point1.y - point2.y), (point1.x - point2.x)) * (180/Math.PI);

        // calculate new angle
        var point1 = points[0].current;
        var point2 = points[1].current;
        var currentAngle = Math.atan2((point1.y - point2.y), (point1.x - point2.x)) * (180/Math.PI);


        var diffAngle = currentAngle - startAngle;
        if (diffAngle > 180) diffAngle -= 360;
        if (diffAngle < -180) diffAngle += 360;

        // set rotation based on difference between the two angles
        stage.rotation += diffAngle * 0.3;

  recreateAllGraphics();
  stage.update();


    }
  });

  stage.addEventListener("stagemouseup", function(event){

      //if (! event.pointerID) event.pointerID = -1;

      if (stage._fingers[event.pointerID]) {
        delete(stage._fingers[event.pointerID]);
      }

      stage._hasEndedAction = true;

      stage._calculateActiveFingers();

      stage.update()

  });

    window.tryInit();


    createjs.Touch.enable(stage);

    return stage

}
