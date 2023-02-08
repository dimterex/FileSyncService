namespace Common.DatabaseProject.Dto
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("users")]
    public class User
    {
        private const char FOLDER_SEPARATOR = ',';

        [Column("user_id")]
        [Key]
        public int UserId { get; set; }

        [Column("login")]
        public string Login { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("available_folders")]
        public string AvailableFoldersString
        {
            get => string.Join(FOLDER_SEPARATOR, AvailableFolders);
            set => AvailableFolders = value.Split(FOLDER_SEPARATOR).ToList();
        }

        [NotMapped]
        public List<string> AvailableFolders { get; set; }
    }
}
