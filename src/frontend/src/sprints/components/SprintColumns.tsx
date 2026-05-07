import { SprintColumn } from './SprintColumn';

interface SprintColumnsProps {
  sprintId?: string;
}

export function SprintColumns({}: SprintColumnsProps) {
  const columns = [
    { type: 'new' as const, title: 'Новые' },
    { type: 'inProgress' as const, title: 'В процессе' },
    { type: 'done' as const, title: 'Сделаны' },
  ];

  return (
    <div className="sprint-columns">
      {columns.map((column) => (
        <SprintColumn
          key={column.type}
          type={column.type}
          title={column.title}
        />
      ))}
    </div>
  );
}
