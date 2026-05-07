import { TaskItem } from '../types/taskItem';

interface TaskCardProps {
  task: TaskItem;
  onEdit: (task: TaskItem) => void;
  onDelete: (task: TaskItem) => void;
  onDragStart?: (e: React.DragEvent) => void;
}

export function TaskCard({ task, onEdit, onDelete, onDragStart }: TaskCardProps) {
  return (
    <div className="task-card" draggable onDragStart={onDragStart}>
      <div className="task-card-actions">
        <button
          className="task-card-btn"
          title="Редактировать"
          onClick={(e) => { e.stopPropagation(); onEdit(task); }}
        >
          ✏️
        </button>
        <button
          className="task-card-btn"
          title="Удалить"
          onClick={(e) => { e.stopPropagation(); onDelete(task); }}
        >
          🗑️
        </button>
      </div>
      <div className="task-card-name">{task.name}</div>
      {task.description && (
        <div className="task-card-description">{task.description}</div>
      )}
    </div>
  );
}
