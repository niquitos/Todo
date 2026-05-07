import { useState, useEffect, useCallback } from 'react';
import { Sprint, CreateSprintDto, UpdateSprintDto } from '../types/sprint';
import { getSprints, createSprint, updateSprint, deleteSprint } from '../api/sprintsApi';

export function useSprints() {
  const [sprints, setSprints] = useState<Sprint[]>([]);
  const [activeSprintId, setActiveSprintId] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadSprints = useCallback(async () => {
    try {
      setLoading(true);
      const data = await getSprints();
      setSprints(data);
      if (data.length > 0 && !activeSprintId) {
        setActiveSprintId(data[0].id);
      }
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load sprints');
    } finally {
      setLoading(false);
    }
  }, [activeSprintId]);

  useEffect(() => {
    loadSprints();
  }, [loadSprints]);

  const handleCreateSprint = async (dto: CreateSprintDto) => {
    try {
      const newSprint = await createSprint(dto);
      setSprints(prev => [...prev, newSprint]);
      setActiveSprintId(newSprint.id);
      return { success: true };
    } catch (err) {
      return {
        success: false,
        error: err instanceof Error ? err.message : 'Failed to create sprint'
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
        error: err instanceof Error ? err.message : 'Failed to update sprint'
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
        error: err instanceof Error ? err.message : 'Failed to delete sprint'
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
