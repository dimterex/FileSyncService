namespace Common.DatabaseProject.Dto
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("history")]
    public class HistoryDto
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("login")]
        public string Login { get; set; }

        [Column("timestamp")]
        public string TimeStamp { get; set; }

        [Column("action")]
        public string Action { get; set; }

        [Column("file")]
        public string File { get; set; }
    }
}
