export interface Sprint {
  id: string;
  name: string;
  startDate: string;
  endDate: string;
  description?: string;
}

export interface CreateSprintDto {
  name: string;
  startDate: string;
  endDate: string;
  description?: string;
}

export interface UpdateSprintDto {
  name: string;
  startDate: string;
  endDate: string;
  description?: string;
}
