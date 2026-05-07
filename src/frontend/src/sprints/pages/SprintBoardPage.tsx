import { useSprints } from '../hooks/useSprints';
import { SprintSelect } from '../components/SprintSelect';
import { SprintForm } from '../components/SprintForm';
import { SprintColumns } from '../components/SprintColumns';
import { useState } from 'react';

export function SprintBoardPage() {
  const {
    sprints,
    activeSprint,
    activeSprintId,
    setActiveSprintId,
    loading,
    error,
    createSprint,
    updateSprint,
    deleteSprint,
  } = useSprints();

  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);

  const handleCreateSuccess = () => {
    setShowCreateModal(false);
  };

  const handleUpdateSuccess = () => {
    setShowEditModal(false);
  };

  if (loading) {
    return <div className="loading">Загрузка...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="sprint-board-page">
      <header className="sprint-board-header">
        <h1>Sprint Board</h1>
        <div className="header-controls">
          <SprintSelect
            sprints={sprints}
            activeSprintId={activeSprintId}
            onSelect={setActiveSprintId}
          />
          <div className="header-actions">
            <button className="btn btn-primary" onClick={() => setShowCreateModal(true)}>
              + Создать спринт
            </button>
            {activeSprint && (
              <>
                <button className="btn btn-secondary" onClick={() => setShowEditModal(true)}>
                  Редактировать
                </button>
                <button className="btn btn-danger" onClick={() => deleteSprint(activeSprint.id)}>
                  Удалить
                </button>
              </>
            )}
          </div>
        </div>
      </header>

      <main className="sprint-board-main">
        <SprintColumns sprintId={activeSprint?.id} />
      </main>

      {showCreateModal && (
        <div className="modal-overlay" onClick={() => setShowCreateModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Создать спринт</h2>
              <button className="modal-close" onClick={() => setShowCreateModal(false)}>&times;</button>
            </div>
            <SprintForm
              onSubmit={createSprint}
              onSuccess={handleCreateSuccess}
            />
          </div>
        </div>
      )}

      {showEditModal && activeSprint && (
        <div className="modal-overlay" onClick={() => setShowEditModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Редактировать спринт</h2>
              <button className="modal-close" onClick={() => setShowEditModal(false)}>&times;</button>
            </div>
            <SprintForm
              sprint={activeSprint}
              onSubmit={(dto) => updateSprint(activeSprint.id, dto)}
              onSuccess={handleUpdateSuccess}
            />
          </div>
        </div>
      )}
    </div>
  );
}
