import { useSprints } from '../hooks/useSprints';
import { SprintBoard } from '../components/SprintBoard';
import '../sprint-board.css';

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

  return (
    <div className="sprint-board-page">
      <h1>Sprint Board</h1>
      <SprintBoard
        sprints={sprints}
        activeSprint={activeSprint}
        activeSprintId={activeSprintId}
        onSelectSprint={setActiveSprintId}
        onCreateSprint={createSprint}
        onUpdateSprint={updateSprint}
        onDeleteSprint={deleteSprint}
        loading={loading}
        error={error}
      />
    </div>
  );
}
