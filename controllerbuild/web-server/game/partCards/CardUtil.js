
var CardUtil = {};

CardUtil.cardGrade = {
    3 : 1,
    4 : 2,
    5 : 3,
    6 : 4,
    7 : 5,
    8 : 6,
    9 : 7,
    10: 8,
    J : 9,
    Q : 10,
    K : 11,
    A : 12,
    2 : 13,
    g : 14,
    G : 15,
};
CardUtil.typeGrade = {
    spade : 4,//黑桃
    hearts : 3,//红桃
    redslice :2,//红方
    blackberry :1,//黑梅
}
CardUtil.typeDown = function(card1,card2){
    return CardUtil.typeGrade[card2.showType]-CardUtil.typeGrade[card1.showType]
}
//降序排列，考慮花色&數字
CardUtil.gradeDown = function(card1,card2){
    var Numcmp = CardUtil.cardGrade[card2.showTxt] - CardUtil.cardGrade[card1.showTxt]
    var Typecmp =  CardUtil.typeGrade[card2.showType]-CardUtil.typeGrade[card1.showType]
    if(Numcmp>0)
        return 1;
    else if(Numcmp == 0 && Typecmp >0)
        return 1;
    else if (Numcmp == 0 && Typecmp < 0 )
        return -1;
  
       return CardUtil.cardGrade[card2.showTxt] - CardUtil.cardGrade[card1.showTxt]
}

//升序排列
CardUtil.gradeUp = function(card1,card2){

    var Numcmp = CardUtil.cardGrade[card1.showTxt] - CardUtil.cardGrade[card2.showTxt]
    var Typecmp =  CardUtil.typeGrade[card1.showType]-CardUtil.typeGrade[card2.showType]
    if(Numcmp>0)
        return 1;
    else if(Numcmp == 0 && Typecmp >0)
        return 1;
    else if (Numcmp == 0 && Typecmp < 0 )
        return -1;
  
       return CardUtil.cardGrade[card1.showTxt] - CardUtil.cardGrade[card2.showTxt]
  
}



module.exports = CardUtil;
