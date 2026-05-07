import { TaskItem, CreateTaskItemDto, UpdateTaskItemDto, MoveTaskItemDto } from '../types/taskItem';

function apiUrl(sprintId: string): string {
  return `/api/sprints/${sprintId}/tasks`;
}

export async function getTaskItems(sprintId: string): Promise<TaskItem[]> {
  const response = await fetch(apiUrl(sprintId));
  if (!response.ok) {
    throw new Error('Failed to fetch task items');
  }
  return response.json();
}

export async function createTaskItem(sprintId: string, dto: CreateTaskItemDto): Promise<TaskItem> {
  const response = await fetch(apiUrl(sprintId), {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Failed to create task item');
  }
  return response.json();
}

export async function updateTaskItem(sprintId: string, taskId: string, dto: UpdateTaskItemDto): Promise<void> {
  const response = await fetch(`${apiUrl(sprintId)}/${taskId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Failed to update task item');
  }
}

export async function deleteTaskItem(sprintId: string, taskId: string): Promise<void> {
  const response = await fetch(`${apiUrl(sprintId)}/${taskId}`, {
    method: 'DELETE',
  });
  if (!response.ok) {
    throw new Error('Failed to delete task item');
  }
}

export async function moveTaskItem(sprintId: string, dto: MoveTaskItemDto): Promise<void> {
  const response = await fetch(`${apiUrl(sprintId)}/move`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Failed to move task item');
  }
}
