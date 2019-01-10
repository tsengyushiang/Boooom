const Player = require('./player');
const dataManager = require('./dataManager');
const DoDizhuRule = require('./DoDizhuRule');
const findtype = require('./findtype');
var global = require('./global');
const countDownSecond = 15;
const Initialpoint = 25;

var GameStep = Object.freeze({ "Waiting": 0, "GameStart": 1, "choseDizhu": 2, "PersonalDouble": 3, "Playing": 4, "Result": 5 });

var AnimationDelay = Object.freeze({
    "None": 0, "GameStart": 2170, "AddDizhuCard": 0, "PASScard": 300,
    "straigth": 830, "Spring": 2080, "reverseSpring": 2080, "airplane": 2000, "bomb": 1500, "laizi": 1500, "rocket": 2920
});

class Room {

    constructor(index) {



        //房間資訊
        this.room = {
            id: index,
            players: [null, null, null] //以玩家名稱作為紀錄

        }

        //開始遊戲資訊
        this.game = {

            //倒數計時器資訊
            timer: {

                stage: GameStep.Waiting,
                countdown: countDownSecond, //目前玩家剩下時間
                whosTurn: 0, //目前輪到哪個玩家
                timer: null, //儲存setInterval Object 用於停止倒數

            },

            //遊戲階段個玩家牌型
            CardsConfig: { //各個玩家得牌
                dizhuIndex: -1,
                who: 0,//目前最大牌是誰出的                
                current: [[], [], []],//目前牌型
                cards: [[], [], []],  //3個玩家的手牌       
                Dizhu: [],   //地主牌
                submitCardTimes: [0, 0, 0] //紀錄玩家出了幾手牌(春天/返春)
            },

            //
            result: {
                //底分
                point: Initialpoint,
                //賠率:含飛機,炸彈,春天...
                Odds: -1,
                //紀錄搶地主階段每個玩家的叫分,-1表示未叫分,0:不要,1:1分,2:2分,3:3分
                playerChosenOdds: [-1, -1, -1],
                //紀錄
                log: {
                    bomb: 1,
                    rocket: 1,
                    RSpring: 1,
                },
                //個人賠率:地主明牌,農民加倍
                personalOdds: [-1, -1, -1],
            }

        }
    }


    //重設遊戲遊戲計數器
    ResetTimer() {

        var game = this.game;

        game.timer.stage = GameStep.Waiting;
        this.game.timer.whosTurn = Math.round(Math.random() * 10) % 3;
        game.timer.countdown = countDownSecond;

        if (game.timer.timer != null)
            clearInterval(this.game.timer.timer);
        game.timer.timer = null;



    };

    //清除上一局牌面
    ResetGame() {

        var game = this.game;


        game.CardsConfig.dizhuIndex = -1;
        game.CardsConfig.who = this.game.timer.whosTurn;
        game.CardsConfig.current = [[], [], []];
        game.CardsConfig.cards = [[], [], []];
        game.CardsConfig.Dizhu = [];
        game.CardsConfig.submitCardTimes = [0, 0, 0];



        game.result.point = Initialpoint;
        game.result.Odds = -1;
        game.result.playerChosenOdds = [-1, -1, -1];
        game.result.log = {
            bomb: 1,
            rocket: 1,
            RSpring: 1,
        };
        game.result.personalOdds = [-1, -1, -1];

    };

    //中斷連線的玩家連回
    LoadGame(uid) {
             
        global.players[uid].socket
            .emit('SwitchScene', 1)
            .emit('roomInfo', dataManager.room(this, global.players[uid].room.playerIndex))
            .emit('playerInfo', dataManager.player(this.room, global.players[uid].room.playerIndex + 3))
            .emit('timer', dataManager.timer(this.game, global.players[uid].room.playerIndex + 3, false))
            .emit('GetCards', dataManager.cards(this.game, global.players[uid].room.playerIndex + 3));

        if (this.game.timer.stage == GameStep.Result) {
            this.showGameResult();              

        }
        
    }

    //玩家是否可加入房間
    Avaiable() {

        if (this.game.timer.stage != GameStep.Waiting)
            return false;
        for (var i = 0; i < 3; i++) {
            if (this.room.players[i] == null) {
                return true;
            }
        }
        return false;
    }

    //房間內新增玩家
    AddPlayer(uid) {

        for (var i = 0; i < 3; i++) {
            if (this.room.players[i] == null) {
                this.room.players[i] = uid;
                global.players[uid].room.playerIndex = i;
                break;
            }
        }

        global.players[uid].room.id = this.room.id;
        global.players[uid].room.Inroom = true;
        console.log(uid, ' is in room ', global.players[uid].room.id);

        var self = this;
        //當玩家滿3人自動開始
        if ((this.room.players[0] != null) && (this.room.players[1] != null) && (this.room.players[2] != null)) {
                self.Gamestart();
           
        }

        self.UpdateRoomInfo();
        self.UpdatePlayerInfo();
        self.Updatetimer();

       
    };

    //移除房間的玩家
    RemovePlayer(uid) {


     
        if (this.game.timer.stage == GameStep.Result) {
            this.room.players[global.players[uid].room.playerIndex] = null;
            global.players[uid].room.Inroom = false;
    
        } else if (this.game.timer.stage != GameStep.Waiting) {
            global.players[uid].IsAI = true; 
        }
        else {
            this.room.players[global.players[uid].room.playerIndex] = null;
           
            global.players[uid].room.Inroom = false;
            this.UpdatePlayerInfo();
        }

        var self = this;
        if ((this.room.players[0] == null) && (this.room.players[1] == null) && (this.room.players[2] == null)) {
            setTimeout(function () {
                self.GameStop();
            },500);
        }

    };

    //更新房間的資訊
    UpdateRoomInfo() {
        for (var i = 0; i < 3; i++) {
            if (this.room.players[i] != null) {
                //傳資料給個player的sokcet
                global.players[this.room.players[i]].socket.emit('roomInfo', dataManager.room(this));
            }
        }

    }

    //更新玩家資訊
    UpdatePlayerInfo() {
        for (var i = 1; i < 4; i++) {
            if (this.room.players[i % 3] != null) {
                //傳資料給個player的sokcet
                global.players[this.room.players[i % 3]].socket.emit('playerInfo', dataManager.player(this.room, i));
            }
        }
    }

    //向client端傳送倒數計時的資料
    Updatetimer(cleanUp = false) {
        for (var i = 0; i < 3; i++) {
            if (this.room.players[i] != null) {
                //傳資料給個player的sokcet
                global.players[this.room.players[i]].socket.emit('timer', dataManager.timer(this.game, i, cleanUp));
            }
        }

    }

    //更新牌組的資訊
    UpdateCardsInfo(cards) {

        for (var i = 1; i < 4; i++) {
            if (this.room.players[i % 3] != null) {
                //傳資料給個player的sokcet
                global.players[this.room.players[i % 3]].socket.emit('GetCards', cards(this.game, i));
            }
        }

    }

    //跳過這個回合
    skipthisTurn() {

        //加倍階段大家都加倍了就跳過
        if (this.game.timer.stage == GameStep.PersonalDouble) {
            for (var i = 0; i < 3; i++) {
                if (this.game.result.personalOdds[i] == -1) {
                    this.callDouble(global.players[this.room.players[i]].uid, 1);
                }
            }
            return;
        }

        //叫分階段要切換到下一個玩家前的自動不叫        
        if ((this.game.timer.stage == GameStep.choseDizhu) && (this.game.result.playerChosenOdds[this.game.timer.whosTurn] == -1)) {
            this.callDizhu(0);
            return;
        }

        //出牌階段要切換到下一個玩家前的自動出牌       
        if ((this.game.timer.stage == GameStep.Playing) && (JSON.stringify(this.game.CardsConfig.current[this.game.timer.whosTurn]) == JSON.stringify([]))) {

            var AutoChoseCard = DoDizhuRule.AutoSubmitCard(
                this.game.CardsConfig.current[this.game.CardsConfig.who],
                this.game.CardsConfig.cards[this.game.timer.whosTurn],
                this.game.timer.whosTurn == this.game.CardsConfig.who);


            this.GetCards(AutoChoseCard);

            if (AutoChoseCard != "PASS")
                return;

        }

        //自動退出遊戲
        if (this.game.timer.stage == GameStep.Result) {
            var self = this;

            this.room.players.forEach(function (playerindex, index) {

                var player = global.players[playerindex]

                if (player == null) return;

                if (player.room.id == self.room.id) {
                    player.Inroom = false;
                    player.socket.emit("SwitchScene", 0);
                    delete global.players[playerindex];
                }

                self.room.players[index] = null;

            });
            self.GameStop();
        }

        //輪到下一個玩家
        this.game.timer.whosTurn++;
        if (this.game.timer.whosTurn > 2) this.game.timer.whosTurn = 0;
        this.game.timer.countdown = countDownSecond;
        this.game.CardsConfig.current[this.game.timer.whosTurn] = [];

        //更新user端資訊
        this.Updatetimer();
        this.UpdateCardsInfo(dataManager.cards);

        //假如PASS一圈回到自己可以出任意牌
        if (this.game.timer.whosTurn == this.game.CardsConfig.who) {
            var self = this;
            //當大家都PASS時不會立即清除牌面，以防未顯示最後一個人的PASS
            self.WaitForAnimations("None", AnimationDelay.PASScard, function () {
                self.game.CardsConfig.current = [[], [], []];
                self.UpdateCardsInfo(dataManager.cards);
            });
        }

    }

    //開始遊戲
    Gamestart() {
        var self = this;
        var GamingObj = this.game;
        var Gamingtimer = this.game.timer;

        //當遊戲一開始時初始化出牌起頭人,跟倒數的秒數

        if (Gamingtimer.stage == GameStep.Waiting) {
            DoDizhuRule.ConfigCard(self.game.CardsConfig); //發牌
            self.UpdateRoomInfo();
            self.game.timer.stage++;
            self.WaitForAnimations("gameStart", AnimationDelay.GameStart, function () {
                self.UpdateCardsInfo(dataManager.cards);//把牌面傳給client
                self.game.timer.stage++;
                self.UpdateRoomInfo();
            });

        }

        self.Updatetimer(); //更新client的timer資訊

        if (Gamingtimer.timer != null)
            clearInterval(Gamingtimer.timer);

        Gamingtimer.timer = setInterval(function () { //將interval儲存,用於停止interval

            Gamingtimer.countdown--;  //開始倒數計時


            //託管自動出牌

            if (global.players[self.room.players[self.game.timer.whosTurn]] != null) {
                if (self.game.timer.stage == GameStep.PersonalDouble) {
                    if (global.players[self.room.players[0]].IsAI)
                        self.callDouble(global.players[self.room.players[0]].uid, 1);
                    if (global.players[self.room.players[1]].IsAI)
                        self.callDouble(global.players[self.room.players[1]].uid, 1);
                    if (global.players[self.room.players[2]].IsAI)
                        self.callDouble(global.players[self.room.players[2]].uid, 1);
                }
                else if (global.players[self.room.players[self.game.timer.whosTurn]].IsAI) {
                    if (self.game.timer.stage == GameStep.choseDizhu) {
                        var point = Math.round(Math.random() * 10) % 4;
                        if (point > self.game.result.Odds)
                            self.callDizhu(point);
                        else
                            self.callDizhu(0);
                    }
                    else if (self.game.timer.stage == GameStep.Playing) {
                        var HintCards = findtype.GetAllType(self.game.CardsConfig.cards[self.game.timer.whosTurn], self.game.CardsConfig.current[self.game.CardsConfig.who]);

                        if (typeof (HintCards) == "undefined") { }
                        else if (HintCards.length == 0) {
                            self.skipthisTurn();
                        }else {

                            var longest = HintCards[0];
                            for (var i = 1; i < HintCards.length; i++) {
                                if (typeof (HintCards[i]) != "undefined")
                                if (HintCards[i].length > longest.length) {
                                    longest = HintCards[i];
                                }
                            }
                            self.GetCards(longest);
                        }
                    }
                }
            }



            if (Gamingtimer.countdown == 0) {   //當數道零換下一位玩家
                self.skipthisTurn()
                return;
            }

            self.Updatetimer(); //更新client的timer資訊

        }, 1000); //每1000ms觸發一次

    }

    //清除timer,game資訊
    GameStop(update = true) {
        this.ResetTimer();
        this.ResetGame();

        if (update) {
            this.Updatetimer();
            this.UpdateCardsInfo(dataManager.clearCards);
            this.UpdateRoomInfo();
        }
    }

    //暫停等待某秒數後做func:After
    WaitForAnimations(Animation, delaySecond, After) {

        var self = this;
        this.GamePause();


        for (var i = 0; i < 3; i++) {
            if (this.room.players[i] != null) {
                //傳資料給個player的sokcet

                //順子特效在每個人的位置，其餘特效都在場中央
                if (Animation == "straight") {
                    switch (self.game.timer.whosTurn - i) {
                        case 1:
                        case -2:
                            global.players[this.room.players[i]].socket.emit('Animation', Animation, "Next");
                            break;
                        case 0:
                            global.players[this.room.players[i]].socket.emit('Animation', Animation, "Me");
                            break;
                        case -1:
                        case 2:
                            global.players[this.room.players[i]].socket.emit('Animation', Animation, "Pre");
                            break;
                    }
                }
                else {
                    global.players[this.room.players[i]].socket.emit('Animation', Animation, "Me");
                }

            }
        }

        //表演完接著做的function
        setTimeout(function () {

            if (typeof (After) != "undefined") {
                After();
            }
            self.Gamestart();

        }, delaySecond, After);
    }

    //清除timer但不重設目前計數資料
    GamePause() {
        clearInterval(this.game.timer.timer);
        this.Updatetimer(true);
    }

    //取得這回合出牌           
    GetCards(cards) {

        var self = this;
        //回傳是文字狀態
        if (typeof (cards) == "string") {
            this.game.CardsConfig.current[this.game.timer.whosTurn] = cards;
        }
        else {
            //判斷是否為可以出的牌型
            var legal = DoDizhuRule.Islegal(this.game.CardsConfig.current[this.game.CardsConfig.who], cards);

            //可出牌
            if (legal) {

                this.game.CardsConfig.who = this.game.timer.whosTurn;
                this.game.CardsConfig.current[this.game.timer.whosTurn] = cards;
                this.game.CardsConfig.submitCardTimes[this.game.timer.whosTurn]++;

                //把出出來的牌從手牌中剃除
                cards.forEach(function (card) {

                    for (var i = 0; i < self.game.CardsConfig.cards[self.game.timer.whosTurn].length; i++) {
                        if (JSON.stringify(self.game.CardsConfig.cards[self.game.timer.whosTurn][i]) == JSON.stringify(card)) {
                            self.game.CardsConfig.cards[self.game.timer.whosTurn].splice(i, 1);
                            break;
                        }
                    }

                });

                //是否為炸彈/飛機/順子... 紀錄倍率跟執行動畫
                var cardAnimation = "None";
                var cardAnimationDelay = AnimationDelay.None;
                switch (DoDizhuRule.CardType(cards)) {
                    case 12:
                        cardAnimation = "rocket";
                        cardAnimationDelay = AnimationDelay.rocket;
                        this.game.result.log.rocket *= 2;
                        this.game.result.Odds *= 2;
                        this.UpdateRoomInfo();
                        break;
                    case 6:
                        cardAnimation = "straight";
                        cardAnimationDelay = AnimationDelay.straigth;
                        break;
                    case 13:
                    case 14:
                        cardAnimation = "airplane";
                        cardAnimationDelay = AnimationDelay.airplane;
                        break;
                    case 11:
                        cardAnimation = "bomb";
                        cardAnimationDelay = AnimationDelay.bomb;
                        this.game.result.log.bomb *= 2;
                        this.game.result.Odds *= 2;
                        this.UpdateRoomInfo();
                        break;

                }


                self.UpdateCardsInfo(dataManager.cards);
                this.WaitForAnimations(cardAnimation, cardAnimationDelay, function () {

                    //判斷是否結束遊戲
                    if (self.game.CardsConfig.cards[self.game.timer.whosTurn].length == 0) {
                       
                        //地主贏嗎
                        var DizhuWin = (JSON.stringify(self.game.timer.whosTurn) == JSON.stringify(self.game.CardsConfig.dizhuIndex));

                        //判定春天或返春
                        if (DizhuWin) {

                            //春天:農民一張牌未出

                            var SubmitCardstimesEqZero = 0;
                            for (var i = 0; i < 3; i++) {
                                if (self.game.CardsConfig.submitCardTimes[i] == 0)
                                    SubmitCardstimesEqZero++;
                            }

                            if (SubmitCardstimesEqZero == 2) {
                                self.game.result.Odds *= 3;
                                self.game.result.log.RSpring *= 3;;
                                self.WaitForAnimations("spring", AnimationDelay.Spring, function () {
                                    self.showGameResult();
                                });
                            } else {
                                self.WaitForAnimations("None", AnimationDelay.PASScard, function () {
                                    self.showGameResult();
                                });
                            }
                        }
                        else {
                            //反春:地主只出第一手牌
                            if (self.game.CardsConfig.submitCardTimes[self.game.CardsConfig.dizhuIndex] == 1) {
                                self.game.result.Odds *= 3;
                                self.game.result.log.RSpring *= 3;
                                self.WaitForAnimations("Rspring", AnimationDelay.reverseSpring, function () {
                                    self.showGameResult();
                                });
                            }
                            else {
                                self.WaitForAnimations("None", AnimationDelay.PASScard, function () {
                                    self.showGameResult();
                                });
                            }
                        }
                        return;
                    }
                    self.skipthisTurn();
                });
            }
            else {
                global.players[this.room.players[this.game.timer.whosTurn]].socket.emit("Animation", 'illegal', "Me");
            }
        }
    }

    //農民/地主加倍
    callDouble(playerid, factor) {

        //依名字找出對應的player Socket
        var playerIndex = 0;
        for (playerIndex = 0; playerIndex < 3; playerIndex++)
            if (global.players[this.room.players[playerIndex]].uid == playerid) break;

        //設定結算倍率
        this.game.result.personalOdds[playerIndex] = factor;
        this.game.timer.whosTurn = playerIndex;

        //根據玩家是地主或農民顯示動畫
        if (factor == 2) {
            this.GetCards("Double");
            this.UpdateCardsInfo(dataManager.cards);
            this.Updatetimer();
            global.players[this.room.players[playerIndex]].socket.emit("Animation", playerIndex == this.game.CardsConfig.dizhuIndex ? "dizhuDouble" : "farmerDouble", "Me");

        }
        else if (factor == 1) {
            this.GetCards("notDouble");
            this.UpdateCardsInfo(dataManager.cards);
            this.Updatetimer();
        }

        //檢查是否都叫過了
        var allCall = true;
        for (var i = 0; i < 3; i++) {
            if (this.game.result.personalOdds[i] == -1) {
                allCall = false;
            }
        }

        if (allCall) {
            this.game.timer.stage++;
            this.game.timer.countdown = countDownSecond;
            this.game.timer.whosTurn = this.game.CardsConfig.dizhuIndex;
            this.game.CardsConfig.who = this.game.CardsConfig.dizhuIndex;

            this.game.CardsConfig.current = [[], [], []];

            var self = this;
            this.WaitForAnimations("AddDizhuCard", 500, function () {
                self.UpdateCardsInfo(dataManager.cards);

            });
        }

    }

    //遊戲結算
    showGameResult() {

        var self = this;

        var DizhuWin = (JSON.stringify(self.game.CardsConfig.dizhuIndex) == JSON.stringify(self.game.CardsConfig.who));
        self.game.timer.stage = GameStep.Result;
        self.game.timer.countdown = countDownSecond;
        self.Updatetimer();

        self.room.players.forEach(function (playerindex, index) {

            global.players[playerindex].socket.emit("ResultMessage", {
                whoWin: (DizhuWin == true) ? "dizhuWin" : "farmerWin",
                odds: {
                    dizhu: "地主 x " + self.game.result.log.dizhu * (index == self.game.CardsConfig.dizhuIndex ? 2 : 1),
                    bomb: "炸彈 x " + self.game.result.log.bomb,
                    rocket: "火箭 x " + self.game.result.log.rocket,
                    RSpring: "春天/返春 x " + self.game.result.log.RSpring,
                },
                remainCards: {
                    player1: {
                        name: global.players[self.room.players[(self.game.CardsConfig.who + 1) % 3]].uid + (((self.game.CardsConfig.who + 1) % 3) == self.game.CardsConfig.dizhuIndex ? "(地主)" : "(農民)"),
                        card1stRow: self.game.CardsConfig.cards[(self.game.CardsConfig.who + 1) % 3].slice(0, 10),
                        card2ndRow: self.game.CardsConfig.cards[(self.game.CardsConfig.who + 1) % 3].slice(10, 20),
                    },
                    player2: {
                        name: global.players[self.room.players[(self.game.CardsConfig.who + 2) % 3]].uid + (((self.game.CardsConfig.who + 2) % 3) == self.game.CardsConfig.dizhuIndex ? "(地主)" : "(農民)"),
                        card1stRow: self.game.CardsConfig.cards[(self.game.CardsConfig.who + 2) % 3].slice(0, 10),
                        card2ndRow: self.game.CardsConfig.cards[(self.game.CardsConfig.who + 2) % 3].slice(10, 20),
                    }
                },
                Personalodds: {
                    player1: {
                        personalDouble: (self.game.result.personalOdds[0] == 2) ? true : false,
                        name: global.players[self.room.players[0]].uid + (0 == self.game.CardsConfig.dizhuIndex ? "(地主)" : "(農民)"),
                        points: "1.00",
                        IsMe: (index == 0),
                    },
                    player2: {
                        personalDouble: (self.game.result.personalOdds[1] == 2) ? true : false,
                        name: global.players[self.room.players[1]].uid + (1 == self.game.CardsConfig.dizhuIndex ? "(地主)" : "(農民)"),
                        points: "2",
                        IsMe: (index == 1),
                    }, player3: {
                        personalDouble: (self.game.result.personalOdds[2] == 2) ? true : false,
                        name: global.players[self.room.players[2]].uid + (2 == self.game.CardsConfig.dizhuIndex ? "(地主)" : "(農民)"),
                        points: "-50",
                        IsMe: (index == 2),
                    },
                },
                timer: "20"
            });

        });

    }

    //搶地主時依照叫分設定
    callDizhu(factor) {

        var self = this;
        var callthreePoint = false;

        //後面的玩家叫比前者高分
        if (factor > this.game.result.Odds) {

            this.game.CardsConfig.dizhuIndex = this.game.timer.whosTurn;
            this.game.result.Odds = factor;
            this.game.result.playerChosenOdds[this.game.timer.whosTurn] = factor;

            //如果要的是3分直接結束
            if (factor == 3) {
                callthreePoint = true;
            }
        }
        //不叫分
        else if (factor == 0) {
            this.game.result.playerChosenOdds[this.game.timer.whosTurn] = factor;
        }
        //小於前者叫分，不反應
        else {
            return;
        }

        if (factor == 0) {
            this.GetCards("notCall");
        } else if (factor == 1) {
            this.GetCards("onePoint");
        } else if (factor == 2) {
            this.GetCards("twoPoints");
        } else if (factor == 3) {
            this.GetCards("threePoints");
        }

        //檢查是否每個人都叫分了
        var allchosen = true;
        for (var i = 0; i < 3; i++) {
            if (this.game.result.playerChosenOdds[i] == -1)
                allchosen = false;
        }
        this.UpdateRoomInfo();

        //當每個玩家都選過地主後
        if (allchosen || callthreePoint) {
            //大家都不叫分
            self.UpdateCardsInfo(dataManager.cards);
            if (this.game.result.Odds <= 0) {

                this.WaitForAnimations("None", AnimationDelay.PASScard, function () {
                    self.GameStop();
                    self.Gamestart();
                });

            }
            //有人叫分
            else {

                this.game.result.log.dizhu = this.game.result.Odds;
                //初始化計數器 進入出牌階段
                this.game.timer.countdown = countDownSecond;
                this.game.timer.whosTurn = this.game.CardsConfig.dizhuIndex;
                var self = this;

                //初始化地主:加手牌,升倍率                   
                self.game.timer.stage++;
                //新增地主牌
                self.game.CardsConfig.cards[self.game.CardsConfig.dizhuIndex].push(self.game.CardsConfig.Dizhu[0]);
                self.game.CardsConfig.cards[self.game.CardsConfig.dizhuIndex].push(self.game.CardsConfig.Dizhu[1]);
                self.game.CardsConfig.cards[self.game.CardsConfig.dizhuIndex].push(self.game.CardsConfig.Dizhu[2]);
                DoDizhuRule.sortCard(self.game.CardsConfig.cards[self.game.CardsConfig.dizhuIndex]);
                self.game.CardsConfig.who = self.game.CardsConfig.dizhuIndex;
                self.game.CardsConfig.current = [[], [], []];



                this.WaitForAnimations("AddDizhuCard", 500, function () {
                    self.UpdateCardsInfo(dataManager.cards);

                });


            }
        } else {
            this.skipthisTurn();
        }
    }



}

module.exports = Room;