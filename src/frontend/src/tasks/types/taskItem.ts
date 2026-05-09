export type ColumnType = 'New' | 'InProgress' | 'Done';

export interface TaskItem {
  id: string;
  name: string;
  description?: string;
  sprintId: string;
  columnType: ColumnType;
  position: number;
}

export interface CreateTaskItemDto {
  name: string;
  description?: string;
  columnType: ColumnType;
  sprintId?: string;
}

export interface UpdateTaskItemDto {
  name: string;
  description?: string;
  columnType: ColumnType;
  position: number;
  sprintId?: string;
}
