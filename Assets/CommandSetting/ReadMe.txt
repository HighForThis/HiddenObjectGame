CommandFunctions.InitiallizeStartReset(Vector3 Pos,int Speed,int Deviation)
游戏开始前复位方法，传入复位点位，复位速度，距离误差，如果调用需要监听CommandFunctions.IsResetStartFinished标志位是否为true



CommandFunctions.IsResetStartFinished 
游戏开始复位完成标志位，为true时表示复位完成
if(CommandFunctions.IsResetStartFinished){
CommandFunctions.IsResetStartFinished=false;
//开始游戏的逻辑
}



ComFunctions.InitializeEndReset(Vector3 Pos,int Speed,int Deviation)
游戏结束后复位方法，传入复位点位，复位速度，距离误差，如果调用需要监听CommandFunctions. IsResetEndFinished标志位是否为true



CommandFunctions.IsResetEndFinished 
游戏结束复位完成标志位，为true时表示复位完成
if(CommandFunctions. IsResetEndFinished){
CommandFunctions. IsResetEndFinished =false;
//游戏结束逻辑
}



CommandFunctions.IsInReset
判断是否在复位中还是不在复位中，当true时表示在复位中



CommandFunctions.RestartReset()
继续/重新复位方法



CommandFunctions.SpasmAgainRecover()
发生二次痉挛的时候的方法



CommandFunctions.SpasmRecover(Vector3 Pos,int Speed,int Deviation,float time)
发生痉挛时调用的方法，传入的是上一个点坐标，复位速度，距离误差,清除痉挛前的等待时间，需要监听CommandFunctions.IsSpasmFinished和CommandFunctions.IsSpasmAgain



CommandFunctions.IsSpasmFinished
清除痉挛过程是否完成，为true时表示已经完成



CommandFunctions.IsSpasmAgain
是否触发二次痉挛，为true时表示触发二次痉挛



//痉挛触发时
if(DiagnosticStatus.MotStatus.Caution){
//打开痉挛UI界面
CommandFunctions.SpasmRecover();
}
if(CommandFunctions.IsSpasmFinished){
CommandFunctions.IsSpasmFinished=true;
if(CommandFunctions.IsInReset){//在复位中继续复位
CommandFunctions.RestartReset();
}
else{
//继续游戏逻辑
}
}

//二次痉挛触发
if(CommandFunctions.IsSpasmAgain){
CommandFunctions.SpasmAgainRecover();//二次痉挛方法
CommandFunctions.IsSpasmAgain=false;
}

