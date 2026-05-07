import { useState, useEffect, useCallback, useRef } from 'react';
import { TaskItem, CreateTaskItemDto, UpdateTaskItemDto, MoveTaskItemDto } from '../types/taskItem';
import { getTaskItems, createTaskItem, updateTaskItem, deleteTaskItem, moveTaskItem } from '../api/taskItemsApi';

export function useTaskItems(sprintId: string | null) {
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
      setError(err instanceof Error ? err.message : 'Failed to load task items');
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
      const created = await createTaskItem(sprintId, dto);
      setTasks(prev => prev.map(t => t.id === tempId ? created : t));
    } catch (err) {
      setTasks(previousTasksRef.current);
      setError(err instanceof Error ? err.message : 'Failed to create task');
    }
  };

  const update = async (taskId: string, dto: UpdateTaskItemDto) => {
    if (!sprintId) return;
    previousTasksRef.current = tasks;

    setTasks(prev => prev.map(t =>
      t.id === taskId ? { ...t, name: dto.name, description: dto.description } : t
    ));

    try {
      await updateTaskItem(sprintId, taskId, dto);
    } catch (err) {
      setTasks(previousTasksRef.current);
      setError(err instanceof Error ? err.message : 'Failed to update task');
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
      await deleteTaskItem(sprintId, taskId);
    } catch (err) {
      setTasks(previousTasksRef.current);
      setError(err instanceof Error ? err.message : 'Failed to delete task');
    }
  };

  const move = async (taskId: string, newColumnType: string, newPosition: number) => {
    if (!sprintId) return;
    previousTasksRef.current = tasks;

    const taskToMove = tasks.find(t => t.id === taskId);
    if (!taskToMove) return;

    const oldColumnType = taskToMove.columnType;
    const oldPosition = taskToMove.position;

    setTasks(prev => {
      let updated = prev.map(t => ({ ...t }));

      if (oldColumnType === newColumnType) {
        if (newPosition < oldPosition) {
          updated = updated.map(t => {
            if (t.columnType === oldColumnType && t.id !== taskId && t.position >= newPosition && t.position < oldPosition) {
              return { ...t, position: t.position + 1 };
            }
            return t;
          });
        } else if (newPosition > oldPosition) {
          updated = updated.map(t => {
            if (t.columnType === oldColumnType && t.id !== taskId && t.position > oldPosition && t.position <= newPosition) {
              return { ...t, position: t.position - 1 };
            }
            return t;
          });
        }
        updated = updated.map(t => t.id === taskId ? { ...t, position: newPosition } : t);
      } else {
        // Remove from source: shift down
        updated = updated.map(t => {
          if (t.columnType === oldColumnType && t.position > oldPosition) {
            return { ...t, position: t.position - 1 };
          }
          return t;
        });
        // Insert into target: shift up
        updated = updated.map(t => {
          if (t.columnType === newColumnType && t.position >= newPosition) {
            return { ...t, position: t.position + 1 };
          }
          return t;
        });
        updated = updated.map(t =>
          t.id === taskId ? { ...t, columnType: newColumnType as TaskItem['columnType'], position: newPosition } : t
        );
      }

      return updated;
    });

    try {
      const moveDto: MoveTaskItemDto = {
        taskId,
        newColumnType: newColumnType as MoveTaskItemDto['newColumnType'],
        newPosition,
      };
      await moveTaskItem(sprintId, moveDto);
    } catch (err) {
      setTasks(previousTasksRef.current);
      setError(err instanceof Error ? err.message : 'Failed to move task');
    }
  };

  return {
    tasks,
    loading,
    error,
    create,
    update,
    remove,
    move,
    refresh: loadTasks,
  };
}
