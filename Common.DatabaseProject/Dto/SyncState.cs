using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.DatabaseProject.Dto
{
    [Table("sync_states")]
    public class SyncState
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("login")]
        public string Login { get; set; }

        [Column("file_path")]
        public string FilePath { get; set; }
    }
}