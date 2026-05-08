import { useState, useEffect, useCallback, useRef } from 'react';
import { TaskItem, CreateTaskItemDto, UpdateTaskItemDto } from '../types/taskItem';
import { getTaskItems, createTaskItem, updateTaskItem, deleteTaskItem } from '../api/taskItemsApi';

export function useTaskItems(sprintId: string | null, onSprintChange?: (sprintId: string) => void) {
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const previousTasksRef = useRef<TaskItem[]>([]);

  const loadTasks = useCallback(async () => {
    if (!sprintId) {
      setTasks([]);
      return;
    }
    try {
      setLoading(true);
      const data = await getTaskItems(sprintId);
      setTasks(data);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Не удалось загрузить задачи');
    } finally {
      setLoading(false);
    }
  }, [sprintId]);

  useEffect(() => {
    loadTasks();
  }, [loadTasks]);

  const create = async (dto: CreateTaskItemDto) => {
    if (!sprintId) return;
    previousTasksRef.current = tasks;

    const tempId = crypto.randomUUID();
    const optimisticTask: TaskItem = {
      id: tempId,
      name: dto.name,
      description: dto.description,
      sprintId,
      columnType: dto.columnType,
      position: 0,
    };

    setTasks(prev => {
      const columnTasks = prev
        .filter(t => t.columnType === dto.columnType)
        .map(t => ({ ...t, position: t.position + 1 }));
      const otherTasks = prev.filter(t => t.columnType !== dto.columnType);
      return [...otherTasks, ...columnTasks, optimisticTask];
    });

    try {
      const created = await createTaskItem({ ...dto, sprintId });
      setTasks(prev => prev.map(t => t.id === tempId ? created : t));
      loadTasks();
      if (dto.sprintId && dto.sprintId !== sprintId) {
        onSprintChange?.(dto.sprintId);
      }
    } catch (err) {
      setTasks(previousTasksRef.current);
      setError(err instanceof Error ? err.message : 'Не удалось создать задачу');
    }
  };

  const update = async (taskId: string, dto: UpdateTaskItemDto) => {
    if (!sprintId) return;
    previousTasksRef.current = tasks;

    setTasks(prev => prev.map(t =>
      t.id === taskId ? { ...t, name: dto.name, description: dto.description } : t
    ));

    try {
      await updateTaskItem(taskId, {
        name: dto.name,
        description: dto.description,
        columnType: dto.columnType,
        position: dto.position,
        sprintId: dto.sprintId || undefined,
      });
      loadTasks();
      if (dto.sprintId && dto.sprintId !== sprintId) {
        onSprintChange?.(dto.sprintId);
      }
    } catch (err) {
      setTasks(previousTasksRef.current);
      setError(err instanceof Error ? err.message : 'Не удалось обновить задачу');
    }
  };

  const remove = async (taskId: string) => {
    if (!sprintId) return;
    previousTasksRef.current = tasks;

    const taskToRemove = tasks.find(t => t.id === taskId);
    setTasks(prev => {
      const filtered = prev.filter(t => t.id !== taskId);
      if (taskToRemove) {
        return filtered.map(t =>
          t.columnType === taskToRemove.columnType && t.position > taskToRemove.position
            ? { ...t, position: t.position - 1 }
            : t
        );
      }
      return filtered;
    });

    try {
      await deleteTaskItem(taskId);
    } catch (err) {
      setTasks(previousTasksRef.current);
      setError(err instanceof Error ? err.message : 'Не удалось удалить задачу');
    }
  };

  return {
    tasks,
    loading,
    error,
    create,
    update,
    remove,
    refresh: loadTasks,
  };
}
