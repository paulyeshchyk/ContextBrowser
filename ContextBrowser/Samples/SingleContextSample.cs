﻿namespace ContextBrowser.Samples.SingleContextSample;

// context: create, tasking
public class TaskService
{
    // context: validate, tasking
    public bool ValidateTask(TaskModel task)
    {
        return new TaskValidator().IsValid(task);
    }

    // context: build, tasking
    public TaskDto BuildDto(TaskModel model)
    {
        return new TaskDtoBuilder().From(model);
    }

    // context: share, notification
    public void Notify(TaskDto dto)
    {
        new Notifier().Send("task.created", dto);
    }

    // context: create, tasking
    public void CreateNewTask(TaskModel task)
    {
        if(!ValidateTask(task))
            return;

        var dto = BuildDto(task);
        new Repository().Save(dto);
        Notify(dto);
    }
}

// context: validate, tasking
public class TaskValidator
{
    public bool IsValid(TaskModel task)
    {
        return !string.IsNullOrWhiteSpace(task.Title);
    }
}

// context: build, tasking
public class TaskDtoBuilder
{
    public TaskDto From(TaskModel model)
    {
        return new TaskDto { Title = model.Title };
    }
}

// context: share, notification
public class Notifier
{
    public void Send(string message, TaskDto dto)
    {
        Console.WriteLine($"Notified: {message}");
    }
}

// context: create, tasking
public class Repository
{
    public void Save(TaskDto dto)
    {
        Console.WriteLine("Task saved");
    }
}

// context: model, tasking
public class TaskModel
{
    public string Title { get; set; } = string.Empty;
}

// context: model, tasking
public class TaskDto
{
    public string Title { get; set; } = string.Empty;
}