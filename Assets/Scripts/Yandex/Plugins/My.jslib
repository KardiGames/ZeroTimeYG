mergeInto(LibraryManager.library, {

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

  CallLoadingApiReadyExtern: function() {
    ysdk.features.LoadingAPI.ready();
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
            console.log('Unity Rewarded!');
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
      console.log ('Unity is handling purchase. Token:'+token);
			myGameInstance.SendMessage('YandexGameObject', 'VipBoughtCallback', token);
		}).catch(err => {
			// Покупка не удалась: в Консоли разработчика не добавлен товар с таким id,
			// пользователь не авторизовался, передумал и закрыл окно оплаты,
			// истекло отведенное на покупку время, не хватило денег и т. д.
			console.log('Unity buy VIP error.');
		});
  },

  ConsumeLostPurchasesExtern: function () {
    console.log('Unity is handling lost purchase on start');
	payments
      .getPurchases()
      .then(purchases => purchases
      .forEach(purchase => {
        var token = purchase.purchaseToken;
        console.log ('Token:' + token);
        myGameInstance.SendMessage('YandexGameObject', 'VipBoughtCallback', token);
      })
    ).catch(err => {
      console.log('Unity FAILED get+consume purchases on start');
    });
	console.log('Unity has finished handling lost purchase some how');
  },

  RequestVipPriceExtern: function () {
    console.log('Unity has requested vip price');
	payments
      .getCatalog()
      .then(products => products
      .forEach(procuct => {
        var priceText=product.price;
		myGameInstance.SendMessage('YandexGameObject', 'SetVipPrice', priceText);
		console.log('Unity has got '+priceText+' as price text');
      })
    ).catch(err => {
      console.log('Unity FAILED set price+currancy for VIP');
    });
  },

  ConsumeTokenExtern: function (tokenString) {
    tokenString=UTF8ToString(tokenString);
    console.log ('Unity is consuming purchase. Token:'+tokenString);
    payments.consumePurchase(tokenString);
  },  
  
});