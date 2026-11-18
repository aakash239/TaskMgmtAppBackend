namespace TaskManagement.Core.Enums;

public enum ProjectRole : byte
{
    Member =0, // can view the project for now 
    Admin = 1 // can view, update, move to trash and restore
}