import { useState, FormEvent } from 'react';
import { TaskItem, CreateTaskItemDto, UpdateTaskItemDto, ColumnType } from '../types/taskItem';
import { Sprint } from '../../sprints/types/sprint';

interface TaskFormProps {
  task?: TaskItem | null;
  columnType?: ColumnType;
  sprints: Sprint[];
  defaultSprintId?: string;
  onSubmit: (dto: CreateTaskItemDto | UpdateTaskItemDto) => Promise<void>;
  onSuccess: () => void;
  onCancel: () => void;
}

const COLUMN_TYPE_OPTIONS: { value: ColumnType; label: string }[] = [
  { value: 'New', label: 'Новые' },
  { value: 'InProgress', label: 'В процессе' },
  { value: 'Done', label: 'Сделаны' },
];

export function TaskForm({ task, columnType, sprints, defaultSprintId, onSubmit, onSuccess, onCancel }: TaskFormProps) {
  const [name, setName] = useState(task?.name || '');
  const [description, setDescription] = useState(task?.description || '');
  const [status, setStatus] = useState<ColumnType>(task?.columnType || columnType || 'New');
  const [sprintId, setSprintId] = useState<string>(task?.sprintId || defaultSprintId || '');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    if (!name.trim()) {
      setError('Название обязательно');
      return;
    }

    setSubmitting(true);
    setError(null);

    try {
      await onSubmit({
        name: name.trim(),
        description: description.trim() || undefined,
        columnType: status,
        position: 0,
        sprintId: sprintId || undefined,
      });
      onSuccess();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ошибка сохранения');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="task-form">
      <div className="form-group">
        <label htmlFor="task-name">Название *</label>
        <input
          id="task-name"
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
          placeholder="Название задачи"
          disabled={submitting}
        />
      </div>

      <div className="form-group">
        <label htmlFor="task-sprint">Спринт</label>
        <select
          id="task-sprint"
          value={sprintId}
          onChange={(e) => setSprintId(e.target.value)}
          disabled={submitting}
        >
          {sprints.map((s) => (
            <option key={s.id} value={s.id}>{s.name}</option>
          ))}
        </select>
      </div>

      <div className="form-group">
        <label htmlFor="task-status">Статус</label>
        <select
          id="task-status"
          value={status}
          onChange={(e) => setStatus(e.target.value as ColumnType)}
          disabled={submitting}
        >
          {COLUMN_TYPE_OPTIONS.map((opt) => (
            <option key={opt.value} value={opt.value}>{opt.label}</option>
          ))}
        </select>
      </div>

      <div className="form-group">
        <label htmlFor="task-description">Описание</label>
        <textarea
          id="task-description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          placeholder="Описание задачи"
          rows={3}
          disabled={submitting}
        />
      </div>

      {error && <div className="form-error">{error}</div>}

      <div className="form-actions">
        <button type="submit" disabled={submitting}>
          {submitting ? 'Сохранение...' : (task ? 'Обновить' : 'Создать')}
        </button>
        <button type="button" onClick={onCancel} disabled={submitting}>
          Отмена
        </button>
      </div>
    </form>
  );
}
