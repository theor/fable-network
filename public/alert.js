
localStorage.debug = null;// "bugout";

// var dispatch;
function callback(f) {
  console.log("callback", f);
  // dispatch = f;
}
var bugoutInstance = null;

function host() {
  bugoutInstance = new Bugout({seed:localStorage["bugout-server-seed"]});
  localStorage["bugout-server-seed"] = bugoutInstance.seed;
  let addr = bugoutInstance.address();
  console.log("hosting, address:", addr);
  bugoutInstance.register("ping", function(address, args, callback) {
    // modify the passed arguments and reply
    console.log("ping from ", address);
    args.hello = "Hello from " + bugoutInstance.address();
    callback(args);
  });
  return addr;
}

function connect(addr) {
  bugoutInstance = new Bugout(addr);
  // wait until we see the server
  // (can take a minute to tunnel through firewalls etc.)
  bugoutInstance.on("server", function (address) {
    bugoutInstance.rpc("ping", { "hello": "world" }, function (result) {
      console.log(result);
      // also check result.error
    });
  });

  // save this client instance's session key seed to re-use
  localStorage["bugout-seed"] = JSON.stringify(bugoutInstance.seed);
  return bugoutInstance.address();
}


// var Bugout = require("bugout");
// var b = new Bugout("test");
// console.log(b.address());
// b.heartbeat(500);

// b.register("asd", function(address, args, cb) {
//   args["pong"] = Math.random();
//   cb(args);
// });

// b.on("timeout", function(address) {
//   console.log("timeout", address);
// });
// b.on("left", function(address) {
//   console.log("left", address);
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

function send(message) {
  // toJSON strips the F# union fields (tag:1, fields=[], ...) and breaks the deserialization
  message.toJSON = undefined;
  console.log("sending", message);
  // b.rpc("ping", {"Hello": "world"}, x => console.log("response:", x));
  // b.send(message);
}

export { host, connect, send, callback };