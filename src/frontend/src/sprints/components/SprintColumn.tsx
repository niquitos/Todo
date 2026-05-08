import { useRef } from 'react';
import { TaskItem, ColumnType } from '../../tasks/types/taskItem';
import { TaskCard } from '../../tasks/components/TaskCard';

interface SprintColumnProps {
  title: string;
  type: 'new' | 'inProgress' | 'done';
  columnType: ColumnType;
  tasks: TaskItem[];
  onAddClick: (columnType: ColumnType) => void;
  onEdit: (task: TaskItem) => void;
  onDelete: (task: TaskItem) => void;
  onDragStart: (e: React.DragEvent, taskId: string) => void;
  onDrop: (e: React.DragEvent, columnType: ColumnType) => void;
}

export function SprintColumn({
  title,
  type,
  columnType,
  tasks,
  onAddClick,
  onEdit,
  onDelete,
  onDragStart,
  onDrop,
}: SprintColumnProps) {
  const contentRef = useRef<HTMLDivElement>(null);

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    e.currentTarget.classList.add('drag-over');
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.currentTarget.classList.remove('drag-over');
  };

  const handleDrop = (e: React.DragEvent) => {
    e.currentTarget.classList.remove('drag-over');
    onDrop(e, columnType);
  };

  return (
    <div className={`sprint-column column-${type}`}>
      <div className="sprint-column-header">
        <h4>{title}</h4>
        <span className="column-counter">{tasks.length}</span>
        <button
          className="column-add-btn"
          title="Добавить задачу"
          onClick={() => onAddClick(columnType)}
        >
          +
        </button>
      </div>
      <div
        ref={contentRef}
        className="sprint-column-content"
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
      >
        {tasks.length === 0 ? (
          <div className="column-placeholder">
            <span>Пусто</span>
          </div>
        ) : (
          tasks.map((task) => (
            <TaskCard
              key={task.id}
              task={task}
              onEdit={onEdit}
              onDelete={onDelete}
              onDragStart={(e) => onDragStart(e, task.id)}
            />
          ))
        )}
      </div>
    </div>
  );
}
