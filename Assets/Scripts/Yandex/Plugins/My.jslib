mergeInto(LibraryManager.library, {

  RequestPlayerData: function () {
    myGameInstance.SendMessage('YandexGameObject', 'SetName', player.getName());
    myGameInstance.SendMessage('YandexGameObject', 'SetPhoto', player.getPhoto("medium"));
  },

  RequestPlayerName: function () {
    myGameInstance.SendMessage('YandexGameObject', 'SetSkillinfoName', player.getName());
  },

  SaveExtern: function(jsonString){
      var jsString = UTF8ToString(jsonString);
      var saveObject = JSON.parse(jsString);
      player.setData(saveObject);
  },

  LoadExtern: function(){
      player.getData().then(jsonString => {
          const saveObject = JSON.stringify(jsonString);
          myGameInstance.SendMessage('Progress', 'SetPlayerInfo', saveObject);
      });
  },

});