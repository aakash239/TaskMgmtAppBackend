namespace TaskManagement.Core.Configuration;

public class ProjectSettings
{
     public const string SectionName = "Projects";

     // number of days the project stays in the trash before it is permanently deleted 
     public int TrashRetentionDays {get; set;} = 30;

     // how often [in hours] it is checked for expired trash 
     public int PurgeIntervalHours {get; set;} =24;



}