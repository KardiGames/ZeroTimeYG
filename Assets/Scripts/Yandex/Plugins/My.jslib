mergeInto(LibraryManager.library, {

  Hello: function () {
    window.alert("Hello, world!");
    console.log("Hello console");
  },

  RequestPlayerData: function () {
    myGameInstance.SendMessage('YandexGameObject', 'SetName', player.getName());
    myGameInstance.SendMessage('YandexGameObject', 'SetPhoto', player.getPhoto("medium"));
  },

});