CommandFunctions.InitiallizeStartReset(Vector3 Pos,int Speed,int Deviation)
��Ϸ��ʼǰ��λ���������븴λ��λ����λ�ٶȣ����������������Ҫ����CommandFunctions.IsResetStartFinished��־λ�Ƿ�Ϊtrue



CommandFunctions.IsResetStartFinished 
��Ϸ��ʼ��λ��ɱ�־λ��Ϊtrueʱ��ʾ��λ���
if(CommandFunctions.IsResetStartFinished){
CommandFunctions.IsResetStartFinished=false;
//��ʼ��Ϸ���߼�
}



ComFunctions.InitializeEndReset(Vector3 Pos,int Speed,int Deviation)
��Ϸ������λ���������븴λ��λ����λ�ٶȣ����������������Ҫ����CommandFunctions. IsResetEndFinished��־λ�Ƿ�Ϊtrue



CommandFunctions.IsResetEndFinished 
��Ϸ������λ��ɱ�־λ��Ϊtrueʱ��ʾ��λ���
if(CommandFunctions. IsResetEndFinished){
CommandFunctions. IsResetEndFinished =false;
//��Ϸ�����߼�
}



CommandFunctions.IsInReset
�ж��Ƿ��ڸ�λ�л��ǲ��ڸ�λ�У���trueʱ��ʾ�ڸ�λ��



CommandFunctions.RestartReset()
����/���¸�λ����



CommandFunctions.SpasmAgainRecover()
�������ξ��ε�ʱ��ķ���



CommandFunctions.SpasmRecover(Vector3 Pos,int Speed,int Deviation,float time)
��������ʱ���õķ��������������һ�������꣬��λ�ٶȣ��������,�������ǰ�ĵȴ�ʱ�䣬��Ҫ����CommandFunctions.IsSpasmFinished��CommandFunctions.IsSpasmAgain



CommandFunctions.IsSpasmFinished
������ι����Ƿ���ɣ�Ϊtrueʱ��ʾ�Ѿ����



CommandFunctions.IsSpasmAgain
�Ƿ񴥷����ξ��Σ�Ϊtrueʱ��ʾ�������ξ���



//���δ���ʱ
if(DiagnosticStatus.MotStatus.Caution){
//�򿪾���UI����
CommandFunctions.SpasmRecover();
}
if(CommandFunctions.IsSpasmFinished){
CommandFunctions.IsSpasmFinished=true;
if(CommandFunctions.IsInReset){//�ڸ�λ�м�����λ
CommandFunctions.RestartReset();
}
else{
//������Ϸ�߼�
}
}

//���ξ��δ���
if(CommandFunctions.IsSpasmAgain){
CommandFunctions.SpasmAgainRecover();//���ξ��η���
CommandFunctions.IsSpasmAgain=false;
}

