import { SprintColumn } from './SprintColumn';
import { TaskItem, ColumnType } from '../../tasks/types/taskItem';

const COLUMN_DEFS = [
  { type: 'new' as const, title: 'Новые', columnType: 'New' as ColumnType },
  { type: 'inProgress' as const, title: 'В процессе', columnType: 'InProgress' as ColumnType },
  { type: 'done' as const, title: 'Сделаны', columnType: 'Done' as ColumnType },
];

interface SprintColumnsProps {
  sprintId?: string;
  tasks: TaskItem[];
  onAddClick: (columnType: ColumnType) => void;
  onEdit: (task: TaskItem) => void;
  onDelete: (task: TaskItem) => void;
  onDragStart: (e: React.DragEvent, taskId: string) => void;
  onDrop: (e: React.DragEvent, columnType: ColumnType, targetPosition: number) => void;
}

export function SprintColumns({
  tasks,
  onAddClick,
  onEdit,
  onDelete,
  onDragStart,
  onDrop,
}: SprintColumnsProps) {
  return (
    <div className="sprint-columns">
      {COLUMN_DEFS.map((col) => {
        const columnTasks = tasks
          .filter((t) => t.columnType === col.columnType)
          .sort((a, b) => a.position - b.position);

        const handleDrop = (e: React.DragEvent, targetColumnType: ColumnType) => {
          const contentEl = e.currentTarget;
          const cards = contentEl.querySelectorAll('.task-card');
          const mouseY = e.clientY;
          let targetPosition = columnTasks.length;

          for (let i = 0; i < cards.length; i++) {
            const rect = cards[i].getBoundingClientRect();
            const midY = rect.top + rect.height / 2;
            if (mouseY < midY) {
              targetPosition = i;
              break;
            }
          }

          onDrop(e, targetColumnType, targetPosition);
        };

        return (
          <SprintColumn
            key={col.type}
            type={col.type}
            title={col.title}
            columnType={col.columnType}
            tasks={columnTasks}
            onAddClick={onAddClick}
            onEdit={onEdit}
            onDelete={onDelete}
            onDragStart={onDragStart}
            onDrop={handleDrop}
          />
        );
      })}
    </div>
  );
}
