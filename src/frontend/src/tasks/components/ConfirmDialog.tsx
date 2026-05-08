interface ConfirmDialogProps {
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  onConfirm: () => void;
  onCancel: () => void;
}

export function ConfirmDialog({
  message,
  confirmLabel = 'Удалить',
  cancelLabel = 'Отмена',
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  return (
    <div className="modal-overlay" onClick={onCancel}>
      <div className="modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Подтверждение</h2>
          <button className="modal-close" onClick={onCancel}>&times;</button>
        </div>
        <div className="confirm-dialog-body">
          <p>{message}</p>
          <div className="form-actions">
            <button className="btn btn-danger" onClick={onConfirm}>
              {confirmLabel}
            </button>
            <button className="btn btn-secondary" onClick={onCancel}>
              {cancelLabel}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
