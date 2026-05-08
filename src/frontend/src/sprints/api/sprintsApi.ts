import { Sprint, CreateSprintDto, UpdateSprintDto } from '../types/sprint';

const API_URL = '/api/sprints';

export async function getSprints(): Promise<Sprint[]> {
  const response = await fetch(API_URL);
  if (!response.ok) {
    throw new Error('Не удалось загрузить спринты');
  }
  return response.json();
}

export async function getSprintById(id: string): Promise<Sprint> {
  const response = await fetch(`${API_URL}/${id}`);
  if (!response.ok) {
    throw new Error('Не удалось загрузить спринт');
  }
  return response.json();
}

export async function createSprint(dto: CreateSprintDto): Promise<Sprint> {
  const response = await fetch(API_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Не удалось создать спринт');
  }
  return response.json();
}

export async function updateSprint(id: string, dto: UpdateSprintDto): Promise<void> {
  const response = await fetch(`${API_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Не удалось обновить спринт');
  }
}

export async function deleteSprint(id: string): Promise<void> {
  const response = await fetch(`${API_URL}/${id}`, {
    method: 'DELETE',
  });
  if (!response.ok) {
    throw new Error('Не удалось удалить спринт');
  }
}
