import { Sprint, CreateSprintDto, UpdateSprintDto } from '../types/sprint';

const API_URL = 'http://localhost:5000/api/sprints';

export async function getSprints(): Promise<Sprint[]> {
  const response = await fetch(API_URL);
  if (!response.ok) {
    throw new Error('Failed to fetch sprints');
  }
  return response.json();
}

export async function getSprintById(id: string): Promise<Sprint> {
  const response = await fetch(`${API_URL}/${id}`);
  if (!response.ok) {
    throw new Error('Failed to fetch sprint');
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
    throw new Error(error || 'Failed to create sprint');
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
    throw new Error(error || 'Failed to update sprint');
  }
}

export async function deleteSprint(id: string): Promise<void> {
  const response = await fetch(`${API_URL}/${id}`, {
    method: 'DELETE',
  });
  if (!response.ok) {
    throw new Error('Failed to delete sprint');
  }
}
