
localStorage.debug = null;// "bugout";
// localStorage.debug = "bugout";

var dispatch = null;
function subscribe(dispatchFunction) {
  console.log("callback", dispatchFunction);
  dispatch = dispatchFunction;
}

var lastSnapshot = null;
function saveSnapshot(snapshot){
  console.log("save", snapshot);
  lastSnapshot = snapshot;
}

var bugoutInstance = null;

function host() {
  bugoutInstance = new Bugout({ seed: localStorage["bugout-server-seed"] });
  localStorage["bugout-server-seed"] = bugoutInstance.seed;
  let addr = bugoutInstance.address();
  console.log("hosting, address:", addr);

  bugoutInstance.register("ping", function (address, args, callback) {
    // modify the passed arguments and reply
    console.log("ping from ", address, lastSnapshot);
    // args.hello = "Hello from " + bugoutInstance.address();
    
    callback({snapshot: lastSnapshot});
  });

  bugoutInstance.on("timeout", function (address) {
    console.log("timeout", address);
  });
  bugoutInstance.on("left", function (address) {
    console.log("left", address);
  });

  
  bugoutInstance.on("message", function(address, message) {
    console.log("message from", address, "is", message);
    dispatch(message)
  });
  return addr;
}

function connect(addr) {
  bugoutInstance = new Bugout(addr);
  bugoutInstance.on("message", function(address, message) {
    console.log("message from", address, "is", message);
    dispatch(message)
  });
  // save this client instance's session key seed to re-use
  localStorage["bugout-seed"] = JSON.stringify(bugoutInstance.seed);

  return new Promise(function(resolve, reject) {
    bugoutInstance.once("server", function (address) {
      bugoutInstance.heartbeat(500);
      bugoutInstance.rpc("ping", { "hello": "world" }, function (result) {
        console.log("snapshot:", result);
        resolve({snapshot: result.snapshot, clientAddr:bugoutInstance.address()});
        // also check result.error
      });
    });
  });
}

function send(message) {
  // toJSON strips the F# union fields (tag:1, fields=[], ...) and breaks the deserialization
  message.toJSON = undefined;
  console.log("sending", message);
  // b.rpc("ping", {"Hello": "world"}, x => console.log("response:", x));
  bugoutInstance.send(message);
}


// var Bugout = require("bugout");
// var b = new Bugout("test");
// console.log(b.address());
// b.heartbeat(500);

// b.register("asd", function(address, args, cb) {
//   args["pong"] = Math.random();
//   cb(args);
// });


// // b.on("ping", function(address) {
// //   console.log("ping", address);
// // });

// b.on("seen", function(address) {
//   console.log("seen", address, address == b.address() ? "Me" : "Not me");
// });

// // receive all out-of-band messages from the server
// // or from any other another connected client
// b.on("message", function(address, message) {
//   console.log("message from", address, "is", message);
//   dispatch(message)
// });

export { host, connect, send, subscribe, saveSnapshot };