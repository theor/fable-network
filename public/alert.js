var dispatch;
function callback(f) {
  console.log("callback", f);
  dispatch = f;
}


const someString = "And I Like that!";
localStorage.debug = null;// "bugout";

// var Bugout = require("bugout");
var b = new Bugout("test");
console.log(b.address());

b.on("server", function(address) {
  console.log("server", address);
  // once we can see the server
  // make an API call on it
  b.rpc("ping", {"hello": "world"}, function(result) {
    console.log(result);
    // {"hello": "world", "pong": true}
    // also check result.error
  });
});

b.on("seen", function(address) {
  console.log("seen", address);
});

// receive all out-of-band messages from the server
// or from any other another connected client
b.on("message", function(address, message) {
  console.log("message from", address, "is", message);
  dispatch(message)
});

// setTimeout(function() {
//   b.send({"AAAA":"BBBB"});
// }, 5000);

function send(message) {
  // toJSON strips the F# union fields (tag:1, fields=[], ...) and breaks the deserialization
  message.toJSON = undefined;
  console.log("sending", message);
  b.send(message);
}

export { send, callback, someString };