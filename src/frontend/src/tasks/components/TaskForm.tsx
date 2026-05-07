import { useState, FormEvent } from 'react';
import { TaskItem, CreateTaskItemDto, UpdateTaskItemDto } from '../types/taskItem';

interface TaskFormProps {
  task?: TaskItem | null;
  onSubmit: (dto: CreateTaskItemDto | UpdateTaskItemDto) => Promise<void>;
  onSuccess: () => void;
  onCancel: () => void;
}

export function TaskForm({ task, onSubmit, onSuccess, onCancel }: TaskFormProps) {
  const [name, setName] = useState(task?.name || '');
  const [description, setDescription] = useState(task?.description || '');
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
      await onSubmit({ name: name.trim(), description: description.trim() || undefined });
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
