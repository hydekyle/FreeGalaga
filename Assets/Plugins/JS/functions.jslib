mergeInto(LibraryManager.library, {

  ShareScore: function (puesto, puntos) {
      compartirEnFacebook (puesto, puntos);
  },

  Information: function () {
    openInformation ();
  },

  Gameover: function () {
    gameover ();
  }

});