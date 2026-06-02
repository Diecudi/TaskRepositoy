using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Tasks.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePictureUrl { get; set; }
        public string FullName { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        
        public ICollection<Sprint> Sprints { get; set; }
        public ICollection<WorkItem> WorkItems { get; set; }
    }

    public class Sprint
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<WorkItem> WorkItems { get; set; }
    }

    public enum WorkItemStatus { Backlog, ToDo, InProgress, Done }

    public class WorkItem
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public WorkItemStatus Status { get; set; }
        
        public string AssignedUserId { get; set; }
        public ApplicationUser AssignedUser { get; set; }

        public string TaskType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Priority { get; set; } // Ej: Alta, Media, Baja

        public int? ProjectId { get; set; }
        public Project Project { get; set; }
        
        public int? SprintId { get; set; }
        public Sprint Sprint { get; set; }
        
        public ICollection<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int WorkItemId { get; set; }
        public WorkItem WorkItem { get; set; }
    }
}