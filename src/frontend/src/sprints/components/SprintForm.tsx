import { Sprint } from '../types/sprint';
import { useSprintForm } from '../hooks/useSprintForm';

interface SprintFormProps {
  sprint?: Sprint | null;
  onSubmit: (dto: { name: string; startDate: string; endDate: string; description?: string }) => Promise<{ success: boolean; error?: string }>;
  onSuccess: () => void;
  onCancel?: () => void;
}

export function SprintForm({ sprint, onSubmit, onSuccess, onCancel }: SprintFormProps) {
  const {
    name,
    setName,
    startDate,
    setStartDate,
    endDate,
    setEndDate,
    description,
    setDescription,
    error,
    submitting,
    handleSubmit,
    reset,
  } = useSprintForm({ sprint, onSubmit, onSuccess });

  return (
    <form onSubmit={handleSubmit} className="sprint-form">
      <div className="form-group">
        <label htmlFor="name">Название *</label>
        <input
          id="name"
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
          placeholder="Sprint 1"
          disabled={submitting}
        />
      </div>

      <div className="form-row">
        <div className="form-group">
          <label htmlFor="startDate">Дата начала *</label>
          <input
            id="startDate"
            type="date"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
            required
            disabled={submitting}
          />
        </div>

        <div className="form-group">
          <label htmlFor="endDate">Дата окончания *</label>
          <input
            id="endDate"
            type="date"
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
            required
            disabled={submitting}
          />
        </div>
      </div>

      <div className="form-group">
        <label htmlFor="description">Описание</label>
        <textarea
          id="description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          placeholder="Описание спринта"
          rows={3}
          disabled={submitting}
        />
      </div>

      {error && <div className="form-error">{error}</div>}

      <div className="form-actions">
        <button type="submit" disabled={submitting}>
          {submitting ? 'Сохранение...' : (sprint ? 'Обновить' : 'Создать')}
        </button>
        {onCancel && (
          <button type="button" onClick={() => { reset(); onCancel(); }} disabled={submitting}>
            Отмена
          </button>
        )}
      </div>
    </form>
  );
}
