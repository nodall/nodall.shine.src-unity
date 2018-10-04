
var callFunc = function(addr, params, callbackFunc) {
  var url = "cmd/"+addr

/*
  if (params.length > 0) url = url + "?"
  for (var i = 0; i < params.length; ++i) {
    url = url + "a" + i + "="+params[i]
    if (i != params.length - 1) url = url + "&"
  }*/

  if (params)
    url = url + "?" + encodeURIComponent(JSON.stringify(params))

  if (callbackFunc && typeof callbackFunc === "function") {
    context.ajax.get(url).then(callbackFunc);
  } else {
    context.ajax.get(url);
  }
}


//window.addEventListener ("load", init, false);

//setTimeout(function() {

var window_loc = window.location;
var ws_url = "ws://" + window_loc.host + window_loc.pathname + "wsapi";
/*

var ws_connection = new WebSocket(ws_url);

ws_connection.onopen = function() {
  console.log(" [WEBSOCKET] onopen");
  //var msg = { action: "RequestDB", params: {} };
  var msg = { action: "get", route: "/" };
  //ws_connection.send("alvdajk lf af");
  setTimeout( function() {
    var str = JSON.stringify(msg);
    console.log("Sending "+str);
    ws_connection.send(str);    
  }, 300);
}

ws_connection.onerror = function() {
  console.log(" [WEBSOCKET] onerror");
}

ws_connection.onclose = function() {
  console.log(" [WEBSOCKET] onclose");
}

ws_connection.onmessage = function(event) {
  console.log(" [WEBSOCKET] onmessage: "+event.data);

  var obj = JSON.parse(event.data);
  //execute('updateDB')

  if (obj.action) {
    executeFromRemote(obj)
  }
}
*/

var MessageHub = function(endpoint, onOpenCallback) {
  var that = this
  this.ws_url = endpoint
  this.channels = {}

  this.onOpenCallback = onOpenCallback;

  var connect = function() {

    that.ws_connection = new WebSocket(endpoint)

    that.ws_connection.onopen = function() {
      console.log(" [WEBSOCKET] onopen");
      that.isOpen = true
      if (that.onOpenCallback)
        that.onOpenCallback(that);
    }

    that.ws_connection.onerror = function() {
      that.isOpen = false
      console.log(" [WEBSOCKET] onerror");
      setTimeout(connect(), 100);
    }

    that.ws_connection.onclose = function() {
      if (that.isOpen) {
        that.isOpen = false
        console.log(" [WEBSOCKET] onclose");
        //shout("Websocket closed, reconnecting...");
        setTimeout(connect(), 100);
      }
    }

    that.ws_connection.onmessage = function(event) {
      //console.log(" [WEBSOCKET] onmessage: "+event.data);

      var obj = JSON.parse(event.data);    

      if (obj.channel && that.channels[obj.channel]) {
        that.channels[obj.channel].execute(obj)    
      }
    }
  }

  connect();

}

MessageHub.prototype.subscribe = function(channel) {
  var ch = this.channels[channel]
  if (ch === undefined) {
    ch = new Channel(this, channel)
    this.channels[channel] = ch
  }
  return ch
}

MessageHub.prototype.unsubscribe = function(channel) {
  if (this.channels.indexOf(channel) >= 0) {
    delete this.channels[channel]
  }
}

MessageHub.prototype.sendInternal = function(msgStr) {
  if (this.isOpen) {
    this.ws_connection.send(msgStr)
  } else {
    setTimeout(this.sendInternal, 100, msgStr);
  }  
}

MessageHub.prototype.send = function(msgChannel, msgType, msgData) {

  var msg = { channel: msgChannel, type: msgType, data: msgData }
  var msgStr = typeof angular != 'undefined' ? angular.toJson(msg) : JSON.stringify(msg)

  this.sendInternal(msgStr);
  /*
  if (this.isOpen) {
    this.ws_connection.send(msgStr)
  } else {
    setTimeout(function() {
      if (this.isOpen) {
        this.ws_connection.send(msgStr)
      }      
    })
  }*/
}

var Channel = function(hub, name) {
  this.hub = hub
  this.name = name
  this.handlers = {}
}

Channel.prototype.on = function(msgType, handler) {
  var handlerList = this.handlers[msgType]
  if (handlerList === undefined) {
    handlerList = []
    this.handlers[msgType] = handlerList
  }
  handlerList.push(handler)
  return this
}

Channel.prototype.execute = function(msgObj) {

  console.log("Executing at "+this.name+" "+msgObj.type)
  var handlerList = this.handlers[msgObj.type]
  if (handlerList) {
    for (var i = 0; i < handlerList.length; ++i) {
      handlerList[i](msgObj.data)
    }
  }
}








var server = {
  create: function(route, obj) {
    var str = JSON.stringify({ action: "create", route: route, data: obj });
    console.log("[server-action] "+str)
    ws_connection.send(str);
  },

  update: function(route, obj) {
    var str = JSON.stringify({ action: "update", route: route, data:obj });
    console.log("[server-action] "+str)
    ws_connection.send(str);
  },

  delete: function(route, obj) {
    var str = JSON.stringify({ action: "delete", route: route, data: obj });
    console.log("[server-action] "+str)
    ws_connection.send(str);
  },

  get: function(route, obj) {
    var str = JSON.stringify({ action: "get", route: route, data: obj });
    console.log("[server-action] "+str)
    ws_connection.send(str);
  }
}


/*
var context = {}

context.requestDB = function() {
  callFunc("RequestDB", null, function(response) {
    //execute('updateDB', response.data);
  });
}

context.requestDevices = function() {
  callFunc("RequestDevices")
}

context.connectToDevice = function(ip) {
  callFunc("ConnectToDevice", [ ip ])
}

context.disconnectFromDevice = function() {
  callFunc("DisconnectFromDevice", [])
}

context.requestAddNode = function(uid, type, name) {
  callFunc("RequestAddNode", { uid: uid, type: type, name: name})
}

context.requestRemoveNode = function(uid) {
  callFunc("RequestRemoveNode", { uid: uid })
}

context.requestPreviewDevice = function(ip) {
  callFunc("RequestPreviewDevice", { ip: ip })
}

context.requestPreviewMedia = function(filename) {
  callFunc("RequestPreviewMedia", { filename: filename } )
}

context.toggleGrid = function() {
  callFunc("ToggleGrid")
}

context.setCurPage = function(uid) {
  callFunc("SetCurPage", { uid: uid })
}

context.setCurScreen = function(uid) {
  callFunc("SetCurScreen", { uid: uid })
}

context.requestUpdateBezier = function(uid, bezierStr) {
  callFunc("RequestUpdateBezier", [ uid, encodeURIComponent(bezierStr) ])
}

context.requestUpdateTextureRectangle = function(uid, texRect) {
  callFunc("RequestUpdateTextureRectangle", [ uid, encodeURIComponent(texRect) ])
}

context.requestUpdateLayerMedia = function(uid, medianame) {
  callFunc("RequestUpdateLayerMedia", [ uid, medianame ])
}

context.requestUpdateRectangle = function(uid, texRect) {
  callFunc("RequestUpdateRectangle", [ uid, encodeURIComponent(texRect)])
}

*/
