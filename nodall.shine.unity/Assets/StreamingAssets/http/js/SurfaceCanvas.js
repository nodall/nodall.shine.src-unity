
  myApp.controller('SurfaceCanvasController', function ($scope, $rootScope) {

    if (typeof window.stageSurface === 'undefined') {        
      window.stageSurface = initCanvas("surfaceCanvas")
    }

    $scope.curSurface = "";

     $scope.$on("$destroy", function(){
        delete window.stageSurface
        window.beziers = new Object()
     });

     $rootScope.$on("OnCurSurfaceChanged", function (event, data) {

        //alert('my surface changed '+data.Name+' ['+data.UID+'] ')

        if (data === undefined) return; // || window.isCropEditor) return

        $scope.curSurface = data

        if (window.beziers.hasOwnProperty(data._id)) {
          var keys = Object.keys(window.beziers)
          for (var i = 0; i < keys.length; ++i) {
            var b = window.beziers[keys[i]]
            if (b._id == data._id) {
              if (b._activeFingers == 0) {
                if (b.typeOf == 'Bezier')  
                  b.parse(data.bezier);
                else if (b.typeOf == 'TextureRectangle')
                  b.parse(data.textureRectangle)
              }
              b.enable()
            }
            else b.disable()
          }
          //window.beziers[data.UID].enable()
        }

     })

     $rootScope.$on("OnSurfacesChanged", function(event, data) {

        window.surfaces = data
        if (window.stageSurface === undefined) return

        window.stageSurface.clear()
        var bkg = window.stageSurface.getChildAt(0)
        window.stageSurface.removeAllChildren()
        window.stageSurface.addChild(bkg)
        window.beziers = new Object()
        $scope.recreateView()

        window.stageSurface.update()
     })

    $rootScope.$on("OnToggleCropEditor", function(event, data) {

        window.stageSurface.clear()
        var bkg = window.stageSurface.getChildAt(0)
        window.stageSurface.removeAllChildren()
        window.stageSurface.addChild(bkg)
        window.beziers = new Object()
        $scope.recreateView()

        window.stageSurface.update()

    })

    $scope.recreateView = function() {
      var data = window.surfaces
      var isCropEditor = window.isCropEditor

      for(var i = 0; i < data.length; ++i) {

        var surf = data[i]
        if (!window.beziers.hasOwnProperty(surf._id)) {

          var bez;
          if (isCropEditor) bez = new TextureRectangle()
          else bez = new Bezier()

          bez.surface = surf
          bez._id = surf._id
          window.beziers[surf._id] = bez
          bez.Name = surf.Name
          bez.stage = window.stageSurface
          bez.name = surf.Name + new Date().getMilliseconds()
          window.stageSurface.addChild(bez)

          if (isCropEditor) bez.parse(surf.textureRectangle)
          else bez.parse(surf.bezier)

          var curSurfaceUID = -1
          if (typeof window.curSurface != 'undefined') curSurfaceUID = window.curSurface._id;

          if (curSurfaceUID == surf._id)  bez.enable()
          else bez.disable()

          console.log("creating surface "+surf._id)

          if (isCropEditor) {
            bez.onChange = function(sender, UID, texrect) {
              sender.surface.textureRectangle = texrect
              console.log('onchange '+UID+' '+texrect+" "+sender.surface._id)
              //server.update('/surfaces', sender.surface)
              hub.send('surfaces', 'update', sender.surface)
            }
          } else {
            bez.onChange = function(sender, UID, bezierstr) {
              console.log('onchange bezier '+sender.surface._id+' '+bezierstr)
              sender.surface.bezier = bezierstr
              //server.update('/surfaces', sender.surface)
              hub.send('surfaces', 'update', sender.surface)              
            }
          }
        }
      }
      window.stageSurface.update()
    }

    $scope.recreateView()

  })
