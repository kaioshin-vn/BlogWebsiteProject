namespace Data.Enums
{
    public enum ModeExam
    {
        Free ,
        Exam
    }

    public enum RankPay
    {
        Nomal,
        Ultra,
        Vip
    }

    public enum KindGroup
    {
        Public,//Bất kể ai cũng có thể xem và đăng
        Restricted,//Cần có admin duyệt bài giống như fb, không vào cũng xem được bài đăng
        Private //Chỉ có thể xem ( người tham gia mới xem được )
    }

    public enum Position
    {
		Chief,
		Deputy
	}

}
