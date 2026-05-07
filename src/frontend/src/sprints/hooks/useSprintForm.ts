import { useState } from 'react';
import { Sprint, CreateSprintDto, UpdateSprintDto } from '../types/sprint';

interface UseSprintFormProps {
  sprint?: Sprint | null;
  onSubmit: (dto: CreateSprintDto | UpdateSprintDto) => Promise<{ success: boolean; error?: string }>;
  onSuccess: () => void;
}

export function useSprintForm({ sprint, onSubmit, onSuccess }: UseSprintFormProps) {
  const [name, setName] = useState(sprint?.name || '');
  const [startDate, setStartDate] = useState(
    sprint?.startDate ? new Date(sprint.startDate).toISOString().split('T')[0] : ''
  );
  const [endDate, setEndDate] = useState(
    sprint?.endDate ? new Date(sprint.endDate).toISOString().split('T')[0] : ''
  );
  const [description, setDescription] = useState(sprint?.description || '');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSubmitting(true);

    const dto: CreateSprintDto | UpdateSprintDto = {
      name,
      startDate: new Date(startDate).toISOString(),
      endDate: new Date(endDate).toISOString(),
      description: description || undefined,
    };

    const result = await onSubmit(dto);

    if (result.success) {
      setName('');
      setStartDate('');
      setEndDate('');
      setDescription('');
      onSuccess();
    } else {
      setError(result.error || 'Operation failed');
    }

    setSubmitting(false);
  };

  const reset = () => {
    setName('');
    setStartDate('');
    setEndDate('');
    setDescription('');
    setError(null);
  };

  return {
    name,
    setName,
    startDate,
    setStartDate,
    endDate,
    setEndDate,
    description,
    setDescription,
    error,
    submitting,
    handleSubmit,
    reset,
  };
}
