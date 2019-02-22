
  myApp.controller('PageCanvasController', function ($scope, $rootScope) {

    if (typeof window.pageStage === 'undefined') {        
      window.pageStage = initCanvas("pageCanvas") //, 0.25, 0.25)
      //window.pageStage.scaleY *= -1
    }

    $scope.curSurface = "";

     $scope.$on("$destroy", function(){
        delete window.pageStage
        window.layersShapes = new Object()
     });

     $rootScope.$on("OnCurLayerChanged", function (event, data) {

        if (data == null) return

        $scope.curLayer = data

        if (window.layersShapes.hasOwnProperty(data._id)) {
          var keys = Object.keys(window.layersShapes)
          for (var i = 0; i < keys.length; ++i) {
            var b = window.layersShapes[keys[i]]
            if (b._id == data._id) b.enable()
            else b.disable()
          }
        }

     })

     $rootScope.$on("OnCurLayerMediaChanged", function (event, data) {

        if (data == null) return

        if (window.layersShapes.hasOwnProperty(data._id)) {
          var keys = Object.keys(window.layersShapes)
          for (var i = 0; i < keys.length; ++i) {
            var b = window.layersShapes[keys[i]]
            if (b._id == data._id) {
              b.removeChild(b.image)
              b.setup("previews/"+data.filename+".jpg")
            }
          }
        }

     })


     $rootScope.$on("OnLayersChanged", function(event, data) {

        window.layers = data

        if (window.pageStage === undefined) return

        window.pageStage.clear()
        var bkg = window.pageStage.getChildAt(0)
        window.pageStage.removeAllChildren()
        window.pageStage.addChild(bkg)
        window.layersShapes = new Object()
        $scope.recreateView()
        window.pageStage.update()
     })

    $scope.recreateView = function() {
      var data = window.layers
      console.log("RecreateView with "+JSON.stringify(data))
      console.log("In stage "+window.pageStage.getNumChildren()+" shapes")
      for(var i = 0; i < data.length; ++i) {

        var layer = data[i]
        if (!window.layersShapes.hasOwnProperty(layer._id)) {

          var tr = new TextureRectangle()
          tr.layer = layer
          //tr.setup("Grid.png")
          var media = layer.filename
          console.log("Creating layer "+layer._id+" using media ("+media+")")
          console.log("Media is "+JSON.stringify(media))
          if (layer.text != null && layer.text.active) {
            tr.setup("text/"+layer._id+".png?" + new Date().getTime())  
          } 
          else if (media != null && media!="") {
            //tr.setup("Grid.png")
            tr.setup("previews/"+layer.filename+".jpg")
          } else {
            tr.setup("Grid.png")
          }

          tr._id = layer._id
          window.layersShapes[layer._id] = tr
          tr.Name = layer.Name
          tr.stage = window.pageStage
          tr.name = layer.Name + new Date().getMilliseconds()
          window.pageStage.addChild(tr)
          tr.parse(layer.rectangle)

          var curLayerUID = -1
          if (typeof window.curLayer != 'undefined') curLayerUID = window.curLayer._id

          if (curLayerUID == layer._id)  tr.enable()
          else tr.disable()

          tr.onChange = function(sender, UID, rect) {
            console.log('onchange rectangle in layer '+UID+' '+rect)
            sender.layer.rectangle = rect
            //server.update('/layers', sender.layer)
            hub.send('layers', 'update', sender.layer)
          }
        }
      }
      window.pageStage.update()
    }
    
    $scope.recreateView()

  })

