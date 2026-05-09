import { TaskItem, CreateTaskItemDto, UpdateTaskItemDto } from '../types/taskItem';

const API_URL = '/api/tasks';

export async function getTaskItems(sprintId: string): Promise<TaskItem[]> {
  const response = await fetch(`${API_URL}?sprintId=${encodeURIComponent(sprintId)}`);
  if (!response.ok) {
    throw new Error('Не удалось загрузить задачи');
  }
  return response.json();
}

export async function createTaskItem(dto: CreateTaskItemDto): Promise<TaskItem> {
  const response = await fetch(API_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Не удалось создать задачу');
  }
  return response.json();
}

export async function updateTaskItem(taskId: string, dto: UpdateTaskItemDto): Promise<void> {
  const response = await fetch(`${API_URL}/${taskId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Не удалось обновить задачу');
  }
}

export async function deleteTaskItem(taskId: string): Promise<void> {
  const response = await fetch(`${API_URL}/${taskId}`, {
    method: 'DELETE',
  });
  if (!response.ok) {
    throw new Error('Не удалось удалить задачу');
  }
}
