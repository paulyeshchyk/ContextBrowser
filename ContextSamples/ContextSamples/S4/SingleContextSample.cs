using System;

namespace ContextSamples.ContextSamples.S4;

// context: create, S4
public class TaskService
{
    // context: validate, S4
    public bool ValidateTask(TaskModel task)
    {
        return new TaskValidator().IsValid(task);
    }

    // context: build, S4
    public TaskDto BuildDto(TaskModel model)
    {
        return new TaskDtoBuilder().From(model);
    }

    // context: share, S4
    public void Notify(TaskDto dto)
    {
        new Notifier().Send("task.created", dto);
    }

    // context: create, S4
    public void CreateNewTask(TaskModel task)
    {
        if (!ValidateTask(task))
            return;

        var dto = BuildDto(task);
        new Repository().Save(dto);
        Notify(dto);
    }
}

// context: validate, S4
public class TaskValidator
{
    public bool IsValid(TaskModel task)
    {
        return !string.IsNullOrWhiteSpace(task.Title);
    }
}

// context: build, S4
public class TaskDtoBuilder
{
    public TaskDto From(TaskModel model)
    {
        return new TaskDto { Title = model.Title };
    }
}

// context: share, S4.1
public class Notifier
{
    // context: share, S4.1
    public void Send(string message, TaskDto dto)
    {
        Console.WriteLine($"Notified: {message}");
    }
}

// context: create, S4
public class Repository
{
    public void Save(TaskDto dto)
    {
        Console.WriteLine("Task saved");
    }
}

// context: model, S4
public class TaskModel
{
    public string Title { get; set; } = string.Empty;
}

// context: model, S4
public class TaskDto
{
    public string Title { get; set; } = string.Empty;
}