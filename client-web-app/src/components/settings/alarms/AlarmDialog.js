import React, { useState, useEffect, Fragment } from 'react';
import {
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogContentText,
    DialogTitle,
    TextField,
    MenuItem,
} from '@mui/material';
import ChipInput from '../../../generic-components/ChipInput';

const AlarmDialog = (props) => {
    const { open, onClose, onSubmit, formData, fieldTypes } = props;
    const [newData, setNewData] = useState(null);

    const handleClose = () => {
        onClose(false);
    };

    const handleSave = (e) => {
        e.preventDefault();
        console.log('NEW DATA:', newData);
        onSubmit(newData);
    };

    const handleOnChange = (key, e, type = 'text') => {
        switch (key) {
            case 'field':
                return setNewData({
                    ...newData,
                    field: Number(
                        Object.keys(fieldTypes).find(
                            (k) => fieldTypes[k] === e.target.value
                        )
                    ),
                });
            case 'active':
                return setNewData({
                    ...newData,
                    active: e.target.value === 'true',
                });
            case 'receivers':
                // const emailString = e.target.value;
                // const emails = emailString.replace(/\s/g, '').split(',');
                // return setNewData({ ...newData, receivers: emails });
                return setNewData({ ...newData, receivers: e });
            default:
                return setNewData({
                    ...newData,
                    [key]:
                        type === 'number'
                            ? e.target.valueAsNumber
                            : e.target.value,
                });
        }
    };

    const renderFormFields = () => {
        return (
            <Fragment>
                <TextField
                    margin='dense'
                    id='name'
                    label='Name'
                    type='text'
                    required
                    value={newData.name}
                    fullWidth
                    variant='outlined'
                    onChange={(e) => handleOnChange('name', e, 'text')}
                />
                <TextField
                    margin='dense'
                    id='field'
                    label='Field'
                    required
                    value={fieldTypes[newData.field]}
                    select
                    onChange={(e) => handleOnChange('field', e)}>
                    {Object.keys(fieldTypes).map((option) => (
                        <MenuItem
                            key={`menu-item-field-${fieldTypes[option]}`}
                            value={fieldTypes[option]}>
                            {fieldTypes[option]}
                        </MenuItem>
                    ))}
                </TextField>
                <TextField
                    margin='dense'
                    id='objective'
                    label='Objective'
                    type='text'
                    required
                    value={newData.objective}
                    fullWidth
                    variant='outlined'
                    onChange={(e) => handleOnChange('objective', e, 'text')}
                />
                <TextField
                    margin='dense'
                    id='threshold'
                    label='Threshold'
                    type='number'
                    required
                    value={newData.threshold}
                    fullWidth
                    variant='outlined'
                    onChange={(e) => handleOnChange('threshold', e, 'number')}
                />
                {formData.active && (
                    <TextField
                        margin='dense'
                        id='active'
                        label='Active'
                        required
                        value={newData.active}
                        select
                        onChange={(e) => handleOnChange('active', e)}>
                        {['true', 'false'].map((option) => (
                            <MenuItem
                                key={`menu-item-active-${option}`}
                                value={option}>
                                {option}
                            </MenuItem>
                        ))}
                    </TextField>
                )}
                <ChipInput
                    onChange={(list) => handleOnChange('receivers', list)}
                    defaultTags={newData.receivers}
                />
            </Fragment>
        );
    };

    useEffect(() => {
        setNewData(formData);
    }, [formData]);

    return (
        <Dialog open={open} onClose={handleClose}>
            <DialogTitle>Edit Alarm Details</DialogTitle>
            <DialogContent>
                <DialogContentText style={{ marginBottom: 16 }}>
                    {formData.id
                        ? 'Edit any of the selected alarm details. Press the Save button in order to save any changes.'
                        : 'Enter the following details and press Submit in order to add a new alarm to the system.'}
                </DialogContentText>
                <form
                    id='edit-alarm-form'
                    onSubmit={handleSave}
                    onKeyDown={(e) => e.key === 'Enter' && e.preventDefault()}>
                    {newData && renderFormFields()}
                </form>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose}>Cancel</Button>
                <Button type='submit' form='edit-alarm-form'>
                    {formData.id ? 'Save' : 'Submit'}
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default AlarmDialog;
