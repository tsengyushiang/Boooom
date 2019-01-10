
var global = require('./global');

var GameStep = Object.freeze({ "Waiting": 0, "GameStart": 1, "choseDizhu": 2, "PersonalDouble": 3, "Playing": 4, "Result": 5 });

var dataManager = {

    //I:room.game
    timer: function (game, index, cleanUp) {
        var Info = game.timer;

        var transferData = {
            active: null, //����O�_�i��
            whosTurn: null, //�x����ܦ�m:Me,Next,Pre
            countdownSecond: null, //�˼Ƭ��
        };

        if (cleanUp)
            return transferData;

        //---------------------------------------------��X�p�ƾ���ܪ���m
        switch (Info.whosTurn - index) {
            case 1:
            case -2:
                transferData.whosTurn = 'Next';
                break;
            case 0:
                transferData.whosTurn = 'Me';
                break;
            case -1:
            case 2:
                transferData.whosTurn = 'Pre';
                break;
        }
        //----------------------------------------------- 
        
        //�[��/���[���q�j�a�P�ɶi��
        if (Info.stage == GameStep.PersonalDouble)
            transferData.whosTurn = "Me";

        //����y�X�P�ɽ��쨺�쪱�a�~��ܫ��s
          transferData.active = transferData.whosTurn == "Me" ? Info.stage : null;

        //�缾�[��/���[�����a���A���button
        if ((Info.stage == GameStep.PersonalDouble) && (game.result.personalOdds[index] != -1))
            transferData.active = null;

        transferData.countdownSecond = Info.countdown;
      
        //�C������ܹC���}�l���q����ܺ�a�έ˼�
        if (Info.stage == GameStep.Waiting || Info.stage == GameStep.GameStart) {
            transferData.active = Info.stage;
            transferData.whosTurn = null;
            transferData.countdownSecond=null;
        }

        if (Info.stage == GameStep.Result) {
            transferData.active = Info.stage;
            transferData.whosTurn = "Result";
        }

        return transferData;
    },    

    //I:room
    room: function (Info,index) {
        
        var transferData = {
            playerPoint: null, //�ثe�I��
            currentOdds: null, //�ثe�ߤ�
            odds: null //�ߤ��߲v
        };

        if (Info.game.timer.stage != GameStep.Waiting) {
            transferData.playerPoint = "....";
            transferData.currentOdds = 12345;
            transferData.odds = Info.game.result.Odds;
        }

        return transferData;
    },

    //I:room.room
    player: function (Info, index) {

        var transferData = {
            me: {
                name: null,
                img: null,
                coin:null
            },
            NextRival: {
                name: null,
                img: null,
                coin: "1234"
            },
            PreRival: {
                name: null,
                img: null,
                coin: "5.77"
            },
        };

        if (typeof (transferData.IsAI = global.players[Info.players[index % 3]]) == "undefined") return transferData;

        transferData.IsAI = global.players[Info.players[index % 3]].IsAI;

        transferData.me.name = global.players[Info.players[index % 3]].uid;
        transferData.NextRival.name = Info.players[(index + 1) % 3] == null ? '' : global.players[Info.players[(index + 1) % 3]].uid;
        transferData.PreRival.name = Info.players[(index - 1) % 3] == null ? '' : global.players[Info.players[(index - 1) % 3]].uid;

        transferData.me.img = global.players[Info.players[index % 3]].img;
        transferData.NextRival.img = Info.players[(index + 1) % 3] == null ? null : global.players[Info.players[(index + 1) % 3]].img;
        transferData.PreRival.img = Info.players[(index - 1) % 3] == null ? null : global.players[Info.players[(index - 1) % 3]].img;



        return transferData;
    },

    //I:room.game
    cards: function (game, index) {

        var Info = game.CardsConfig;

        var transferData = {
            DizhuCards: [], //�a�D�P
            MyCards: null, //�ڪ���P
            currentStatus: {
                Me: null,
                Pre: null,
                Next: null
            },//�ثe�P��
            PreRivalCards: 0, //�e�@�쪱�a����P��
            NextRivalCards: 0, //��@�쪱�a����P��
            IsDizhu: {
                Me: null,
                Pre: null,
                Next: null
            }
        }
        
        if (game.timer.stage > GameStep.choseDizhu) {
            transferData.DizhuCards = Info.Dizhu;
            transferData.IsDizhu.Me = Info.dizhuIndex == (index % 3) ? true : null;
            transferData.IsDizhu.Pre = Info.dizhuIndex == ((index - 1) % 3) ? true : null;
            transferData.IsDizhu.Next = Info.dizhuIndex == ((index + 1) % 3) ? true : null;
        }
        else if (game.timer.stage != GameStep.Waiting) {
            transferData.DizhuCards = [null, null, null];
        }

        transferData.currentStatus.Me = Info.current[index % 3];
        transferData.currentStatus.Pre = Info.current[(index - 1) % 3];
        transferData.currentStatus.Next = Info.current[(index + 1) % 3];

        transferData.MyCards = Info.cards[index % 3];

        if ((game.result.personalOdds[game.CardsConfig.dizhuIndex] == 2) && (game.timer.stage > GameStep.PersonalDouble)) {

            if (((index - 1) % 3) == game.CardsConfig.dizhuIndex)
                transferData.PreRivalCards = Info.cards[(index - 1) % 3];
            else
                transferData.PreRivalCards = Info.cards[(index - 1) % 3].length;
            if (((index + 1) % 3) == game.CardsConfig.dizhuIndex)
                transferData.NextRivalCards = Info.cards[(index + 1) % 3];
            else
                transferData.NextRivalCards = Info.cards[(index + 1) % 3].length;

        }
        else {
            transferData.PreRivalCards = Info.cards[(index - 1) % 3].length;
            transferData.NextRivalCards = Info.cards[(index + 1) % 3].length;
        }



        return transferData;
    },
    clearCards: function (Info, index) {
        var transferData = {
            DizhuCards: [],
            MyCards: [],
            currentStatus: {
                Me: [],
                Pre: [],
                Next: []
            },
            PreRivalCards: 0,
            NextRivalCards: 0,
            IsDizhu: {
                Me: null,
                Pre: null,
                Next: null
            }
        }

        return transferData;

    },

};

module.exports = dataManager;