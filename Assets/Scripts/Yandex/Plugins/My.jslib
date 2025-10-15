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

  UnityReady: function() {
      console.log('UNITY calls UnityReady on Start');
  },

  SetScore: function(score){
      ysdk.leaderboards.setScore('Progress', score);
  },

  ShowAd: function () {
    ysdk.adv.showRewardedVideo({
      callbacks: {
          onOpen: () => {
            console.log('Unity AD started.');
          },
          onRewarded: () => {
            console.log('Rewarded!');
            myGameInstance.SendMessage('YandexGameObject', 'AdShownCallback');
          },
          onClose: () => {
            console.log('Unity AD closed.');
            myGameInstance.SendMessage('YandexGameObject', 'AdDidntShowCallback');
          },
          onError: (e) => {
            console.log('Unity AD error. (Must be close message near. Check!):', e);
          },
      }
    })
  },
});