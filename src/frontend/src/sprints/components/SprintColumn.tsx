
interface SprintColumnProps {
  title: string;
  type: 'new' | 'inProgress' | 'done';
  children?: React.ReactNode;
}

export function SprintColumn({ title, type, children }: SprintColumnProps) {
  return (
    <div className={`sprint-column column-${type}`}>
      <div className="sprint-column-header">
        <h4>{title}</h4>
      </div>
      <div className="sprint-column-content">
        {children || (
          <div className="column-placeholder">
            <span>Пусто</span>
          </div>
        )}
      </div>
    </div>
  );
}
