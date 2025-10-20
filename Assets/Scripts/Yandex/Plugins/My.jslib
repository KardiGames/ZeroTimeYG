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

  ShowAdExtern: function () {
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
  
  BuyVipExtern: function () {
	  payments.purchase({ id: 'vipstatus' })
		.then(purchase => {
      var token = purchase.purchaseToken;
      var bufferSize = lengthBytesUTF8(token) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(token, buffer, bufferSize);
			myGameInstance.SendMessage('YandexGameObject', 'VipBoughtCallback', buffer);
		}).catch(err => {
			// Покупка не удалась: в Консоли разработчика не добавлен товар с таким id,
			// пользователь не авторизовался, передумал и закрыл окно оплаты,
			// истекло отведенное на покупку время, не хватило денег и т. д.
			console.log('Unity buy VIP error.');
		});
  },
  
});