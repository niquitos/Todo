import { useState } from 'react';
import { useSprints } from '../hooks/useSprints';
import { useTaskItems } from '../../tasks/hooks/useTaskItems';
import { SprintSelect } from '../components/SprintSelect';
import { SprintForm } from '../components/SprintForm';
import { SprintColumns } from '../components/SprintColumns';
import { TaskForm } from '../../tasks/components/TaskForm';
import { ConfirmDialog } from '../../tasks/components/ConfirmDialog';
import { TaskItem, ColumnType, CreateTaskItemDto, UpdateTaskItemDto } from '../../tasks/types/taskItem';

export function SprintBoardPage() {
  const {
    sprints,
    activeSprint,
    activeSprintId,
    setActiveSprintId,
    loading: sprintsLoading,
    error: sprintsError,
    createSprint,
    updateSprint,
    deleteSprint,
  } = useSprints();

  const {
    tasks,
    loading: tasksLoading,
    error: tasksError,
    create: createTask,
    update: updateTask,
    remove: removeTask,
    move: moveTask,
  } = useTaskItems(activeSprintId || null);

  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);

  // Task modal states
  const [createModalColumn, setCreateModalColumn] = useState<ColumnType | null>(null);
  const [editModalTask, setEditModalTask] = useState<TaskItem | null>(null);
  const [confirmDeleteTask, setConfirmDeleteTask] = useState<TaskItem | null>(null);

  const defaultSprint = sprints.find(s => s.isDefault);
  const defaultSprintId = defaultSprint?.id;

  // Drag-and-drop
  const handleDragStart = (e: React.DragEvent, taskId: string) => {
    e.dataTransfer.setData('text/plain', taskId);
    e.dataTransfer.effectAllowed = 'move';
  };

  const handleDrop = async (e: React.DragEvent, columnType: ColumnType, targetPosition: number) => {
    const taskId = e.dataTransfer.getData('text/plain');
    if (!taskId) return;
    await moveTask(taskId, columnType, targetPosition);
  };

  const handleCreateTask = async (dto: CreateTaskItemDto | UpdateTaskItemDto) => {
    const createDto = dto as CreateTaskItemDto;
    await createTask(createDto);
  };

  const handleUpdateTask = async (dto: CreateTaskItemDto | UpdateTaskItemDto) => {
    if (!editModalTask) return;
    const updateDto = dto as UpdateTaskItemDto;
    await updateTask(editModalTask.id, updateDto);
  };

  const handleDeleteConfirm = async () => {
    if (!confirmDeleteTask) return;
    await removeTask(confirmDeleteTask.id);
    setConfirmDeleteTask(null);
  };

  const handleCreateSuccess = () => {
    setShowCreateModal(false);
  };

  const handleUpdateSuccess = () => {
    setShowEditModal(false);
  };

  const handleTaskCreateSuccess = () => {
    setCreateModalColumn(null);
  };

  const handleTaskUpdateSuccess = () => {
    setEditModalTask(null);
  };

  if (sprintsLoading) {
    return <div className="loading">Загрузка...</div>;
  }

  if (sprintsError) {
    return <div className="error">{sprintsError}</div>;
  }

  return (
    <div className="sprint-board-page">
      <header className="sprint-board-header">
        <h1>Доска спринта</h1>
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
        {tasksLoading && <div className="column-placeholder">Загрузка задач...</div>}
        {tasksError && <div className="form-error">{tasksError}</div>}
        {!tasksLoading && !tasksError && (
          <SprintColumns
            sprintId={activeSprint?.id}
            tasks={tasks}
            onAddClick={(ct) => setCreateModalColumn(ct)}
            onEdit={(task) => setEditModalTask(task)}
            onDelete={(task) => setConfirmDeleteTask(task)}
            onDragStart={handleDragStart}
            onDrop={handleDrop}
          />
        )}
      </main>

      {/* Sprint modals */}
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

      {/* Create task modal */}
      {createModalColumn && (
        <div className="modal-overlay" onClick={() => setCreateModalColumn(null)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Создать задачу</h2>
              <button className="modal-close" onClick={() => setCreateModalColumn(null)}>&times;</button>
            </div>
            <TaskForm
              columnType={createModalColumn}
              sprints={sprints}
              defaultSprintId={activeSprintId || defaultSprintId}
              onSubmit={handleCreateTask}
              onSuccess={handleTaskCreateSuccess}
              onCancel={() => setCreateModalColumn(null)}
            />
          </div>
        </div>
      )}

      {/* Edit task modal */}
      {editModalTask && (
        <div className="modal-overlay" onClick={() => setEditModalTask(null)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Редактировать задачу</h2>
              <button className="modal-close" onClick={() => setEditModalTask(null)}>&times;</button>
            </div>
            <TaskForm
              task={editModalTask}
              sprints={sprints}
              onSubmit={handleUpdateTask}
              onSuccess={handleTaskUpdateSuccess}
              onCancel={() => setEditModalTask(null)}
            />
          </div>
        </div>
      )}

      {/* Confirm delete dialog */}
      {confirmDeleteTask && (
        <ConfirmDialog
          message={`Вы уверены, что хотите удалить задачу «${confirmDeleteTask.name}»?`}
          onConfirm={handleDeleteConfirm}
          onCancel={() => setConfirmDeleteTask(null)}
        />
      )}
    </div>
  );
}
