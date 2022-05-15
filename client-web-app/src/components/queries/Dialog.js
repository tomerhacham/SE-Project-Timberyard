import React from 'react';
import Button from '@mui/material/Button';
import DialogTitle from '@mui/material/DialogTitle';
import Dialog from '@mui/material/Dialog';
import MinMaxChart from './graph/MinMaxChart';

const DialogScreen = (props) => {
    const { onClose, open, data } = props;

    const handleClose = () => {
        onClose();
    };

    return (
        <Dialog id='dialog-box' fullWidth onClose={handleClose} open={open}>
            <DialogTitle>{data.TestName}</DialogTitle>
            <MinMaxChart data={data} />
        </Dialog>
    );
};

const DialogButton = (props) => {
    const [open, setOpen] = React.useState(false);
    const { data } = props;

    const handleClickOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
    };

    return (
        <div>
            <Button
                id='show-graph-button'
                variant='text'
                onClick={handleClickOpen}>
                Show Graph
            </Button>
            <DialogScreen open={open} onClose={handleClose} data={data} />
        </div>
    );
};

export default DialogButton;
