import { useState } from 'react';
import { Sprint } from '../types/sprint';
import { SprintSelect } from './SprintSelect';
import { SprintForm } from './SprintForm';
import { SprintColumns } from './SprintColumns';

interface SprintBoardProps {
  sprints: Sprint[];
  activeSprint: Sprint | null;
  activeSprintId: string | null;
  onSelectSprint: (id: string) => void;
  onCreateSprint: (dto: { name: string; startDate: string; endDate: string; description?: string }) => Promise<{ success: boolean; error?: string }>;
  onUpdateSprint: (id: string, dto: { name: string; startDate: string; endDate: string; description?: string }) => Promise<{ success: boolean; error?: string }>;
  onDeleteSprint: (id: string) => Promise<{ success: boolean; error?: string }>;
  loading: boolean;
  error: string | null;
}

export function SprintBoard({
  sprints,
  activeSprint,
  activeSprintId,
  onSelectSprint,
  onCreateSprint,
  onUpdateSprint,
  onDeleteSprint,
  loading,
  error,
}: SprintBoardProps) {
  const [editingSprint, setEditingSprint] = useState<Sprint | null>(null);

  const handleCreateSuccess = () => {
    // Refresh is handled by parent
  };

  const handleUpdateSuccess = () => {
    setEditingSprint(null);
  };

  const handleDelete = async () => {
    if (activeSprint && confirm(`Удалить спринт "${activeSprint.name}"?`)) {
      await onDeleteSprint(activeSprint.id);
    }
  };

  if (loading) {
    return <div className="loading">Загрузка...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="sprint-board-container">
      <div className="sprint-board-header">
        <SprintSelect
          sprints={sprints}
          activeSprintId={activeSprintId}
          onSelect={onSelectSprint}
        />
        {activeSprint && (
          <button className="btn-delete" onClick={handleDelete}>
            Удалить спринт
          </button>
        )}
      </div>

      <div className="sprint-board-content">
        <div className="sprint-form-section">
          <SprintForm
            onSubmit={onCreateSprint}
            onSuccess={handleCreateSuccess}
          />
        </div>

        {activeSprint && (
          <div className="sprint-details-section">
            {editingSprint ? (
              <SprintForm
                sprint={editingSprint}
                onSubmit={(dto) => onUpdateSprint(editingSprint.id, dto)}
                onSuccess={handleUpdateSuccess}
                onCancel={() => setEditingSprint(null)}
              />
            ) : (
              <div className="sprint-info">
                <h3>Информация о спринте</h3>
                <p><strong>Название:</strong> {activeSprint.name}</p>
                <p><strong>Даты:</strong> {new Date(activeSprint.startDate).toLocaleDateString()} - {new Date(activeSprint.endDate).toLocaleDateString()}</p>
                {activeSprint.description && (
                  <p><strong>Описание:</strong> {activeSprint.description}</p>
                )}
                <button className="btn-edit" onClick={() => setEditingSprint(activeSprint)}>
                  Редактировать
                </button>
              </div>
            )}
          </div>
        )}

        <div className="sprint-columns-container">
          <SprintColumns
            sprintId={activeSprint?.id}
            tasks={[]}
            onAddClick={() => {}}
            onEdit={() => {}}
            onDelete={() => {}}
            onDragStart={() => {}}
            onDrop={() => {}}
          />
        </div>
      </div>
    </div>
  );
}
