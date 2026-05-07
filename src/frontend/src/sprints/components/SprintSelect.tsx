import { Sprint } from '../types/sprint';

interface SprintSelectProps {
  sprints: Sprint[];
  activeSprintId: string | null;
  onSelect: (id: string) => void;
}

export function SprintSelect({ sprints, activeSprintId, onSelect }: SprintSelectProps) {
  return (
    <div className="sprint-select-container">
      <label htmlFor="sprint-select">Выберите спринт:</label>
      <select
        id="sprint-select"
        value={activeSprintId || ''}
        onChange={(e) => onSelect(e.target.value)}
        className="sprint-select"
      >
        {sprints.length === 0 ? (
          <option value="">Нет доступных спринтов</option>
        ) : (
          sprints.map((sprint) => (
            <option key={sprint.id} value={sprint.id}>
              {sprint.name} ({new Date(sprint.startDate).toLocaleDateString()} - {new Date(sprint.endDate).toLocaleDateString()})
            </option>
          ))
        )}
      </select>
    </div>
  );
}
