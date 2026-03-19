# Patient Import Pipeline Bot

## Objective
Create a Medplum bot that processes a FHIR Bundle containing patient demographic data and related observations, validates the data, and imports it into the system.

## Requirements

### Input
- Accept a FHIR Bundle (type: transaction) as input
- Bundle may contain:
  - Patient resources with demographics (name, birthDate, gender, identifiers)
  - Related Observation resources (vitals, lab results)
  - Other linked resources (Practitioner, Organization)

### Processing Logic
1. Validate the incoming bundle structure
2. Check for duplicate patients using identifiers (MRN)
3. For each Patient resource:
   - If patient exists (matching identifier), update the existing record
   - If patient is new, create a new Patient resource
4. Process related Observation resources and link them to the correct Patient
5. Handle errors gracefully and collect them in a report

### Output
Return a JSON object with:
- `success`: boolean indicating overall success
- `processedPatients`: count of patients processed
- `createdPatients`: count of new patients created
- `updatedPatients`: count of existing patients updated
- `processedObservations`: count of observations processed
- `errors`: array of any errors encountered (should not throw, just collect)

## Validation Rules
- Patient must have at least one identifier
- Patient must have a name
- Observations must have a valid status and code
- Observations must reference a valid Patient

## Error Handling
- Continue processing even if individual resources fail
- Collect all errors and return them in the output
- Log all significant operations for debugging

## Example Usage
The bot will be triggered by an external system sending a FHIR Bundle via HTTP POST.
