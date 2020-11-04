mergeInto(LibraryManager.library, {

  Hello: function (name) {
    console.log("Hello " + name);
  },

  AskUsername: function () {
      ShowLogin();
  },

  ReadName: function () {
      return document.getElementById("display_username").value;
  },

});