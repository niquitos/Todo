import { useState, useEffect, useCallback } from 'react';
import { Sprint, CreateSprintDto, UpdateSprintDto } from '../types/sprint';
import { getSprints, createSprint, updateSprint, deleteSprint } from '../api/sprintsApi';

export function useSprints(initialSprintId?: string | null) {
  const [sprints, setSprints] = useState<Sprint[]>([]);
  const [activeSprintId, setActiveSprintId] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [initialized, setInitialized] = useState(false);

  const loadSprints = useCallback(async () => {
    try {
      setLoading(true);
      const data = await getSprints();
      setSprints(data);
      // Установить activeSprintId: из URL или первый из списка
      if (data.length > 0) {
        const targetId = initialSprintId ?? data[0].id;
        setActiveSprintId(targetId);
      }
      setError(null);
      setInitialized(true);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Не удалось загрузить спринты');
      setInitialized(true);
    } finally {
      setLoading(false);
    }
  }, [initialSprintId]);


  // Update URL when active sprint changes (after initial load)
  useEffect(() => {
    if (initialized && activeSprintId) {
      const params = new URLSearchParams(window.location.search);
      params.set('sprint', activeSprintId);
      const newUrl = `${window.location.pathname}${params.toString() ? '?' + params.toString() : ''}`;
      window.history.replaceState({}, '', newUrl);
    }
  }, [activeSprintId, initialized]);

  // Load sprints on mount (only once)
  useEffect(() => {
    loadSprints();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleCreateSprint = async (dto: CreateSprintDto) => {
    try {
      const newSprint = await createSprint(dto);
      setSprints(prev => [...prev, newSprint]);
      setActiveSprintId(newSprint.id);
      return { success: true };
    } catch (err) {
      return {
        success: false,
        error: err instanceof Error ? err.message : 'Не удалось создать спринт'
      };
    }
  };

  const handleUpdateSprint = async (id: string, dto: UpdateSprintDto) => {
    try {
      await updateSprint(id, dto);
      await loadSprints();
      return { success: true };
    } catch (err) {
      return {
        success: false,
        error: err instanceof Error ? err.message : 'Не удалось обновить спринт'
      };
    }
  };

  const handleDeleteSprint = async (id: string) => {
    try {
      await deleteSprint(id);
      setSprints(prev => prev.filter(s => s.id !== id));
      if (activeSprintId === id) {
        setActiveSprintId(sprints.find(s => s.id !== id)?.id || null);
      }
      return { success: true };
    } catch (err) {
      return {
        success: false,
        error: err instanceof Error ? err.message : 'Не удалось удалить спринт'
      };
    }
  };

  const activeSprint = sprints.find(s => s.id === activeSprintId) || null;

  return {
    sprints,
    activeSprint,
    activeSprintId,
    setActiveSprintId,
    loading,
    error,
    createSprint: handleCreateSprint,
    updateSprint: handleUpdateSprint,
    deleteSprint: handleDeleteSprint,
    refresh: loadSprints,
  };
}
